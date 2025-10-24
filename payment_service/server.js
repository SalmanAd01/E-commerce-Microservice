import express from 'express';
import { initializeModels, syncDatabase } from './models/index.js';
import Payment from './models/payment.js';
import swaggerUi from 'swagger-ui-express';
import swaggerJSDoc from 'swagger-jsdoc';
import cors from 'cors';
// swagger-jsdoc configuration: the API docs will be generated from JSDoc comments
const swaggerOptions = {
    definition: {
        openapi: '3.0.0',
        info: {
            title: 'Payment Service API',
            version: '1.0.0',
            description: 'APIs for creating Razorpay orders and verifying payments',
        },
        servers: [{ url: 'http://localhost:8000' }],
    },
    apis: ['./server.js'],
};

const swaggerSpec = swaggerJSDoc(swaggerOptions);

const app = express();
app.use(express.json());
app.use(cors({
    origin: '*',
    methods: ['GET', 'POST', 'PUT', 'DELETE'],
}));

// Swagger UI (API documentation)
app.use('/api-docs', swaggerUi.serve, swaggerUi.setup(swaggerSpec));

app.get('/', (req, res) => {
    res.send('Hello, World ! here we goo');
});

// Create a Razorpay order
/**
 * @swagger
 * /razorpay/order:
 *   post:
 *     summary: Create a Razorpay order
 *     tags:
 *       - Razorpay
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               amount:
 *                 type: number
 *                 example: 12.34
 *               currency:
 *                 type: string
 *                 example: INR
 *               receipt:
 *                 type: string
 *                 example: rcpt_123
 *               payment_capture:
 *                 type: integer
 *                 example: 1
 *               localOrderId:
 *                 type: integer
 *                 description: Your internal order id from the order service — will be stored in the payments DB as orderId
 *             required:
 *               - amount
 *     responses:
 *       200:
 *         description: Razorpay order created
 *         content:
 *           application/json:
 *             schema:
 *               type: object
 *               additionalProperties: true
 */
app.post('/razorpay/order', async (req, res) => {
    const { amount, currency, receipt, payment_capture, localOrderId } = req.body;
    if (!amount) return res.status(400).json({ error: 'amount is required' });

    try {
        const order = await Payment.createRazorpayOrder({ amount, currency, receipt, payment_capture });

        // If the client provided a local order id, persist a pending payment record linking to it
        if (localOrderId != null && localOrderId !== undefined) {
            try {
                await Payment.createPaymentRecord({
                    orderId: localOrderId,
                    paymentMethod: 'razorpay',
                    transactionId: order.id, // razorpay order id
                    amount: amount || (order.amount ? Number(order.amount) / 100 : 0),
                    currency: currency || (order.currency || 'INR'),
                    status: 'pending',
                });
            } catch (dbErr) {
                console.error('Failed to create payment record:', dbErr);
                // Return 500 because user requested DB persistence
                return res.status(500).json({ error: 'Failed to persist payment record', details: dbErr.message });
            }
        }

        res.json(order);
    } catch (error) {
        console.error('Error creating razorpay order:', error);
        res.status(500).json({ error: error.message });
    }
});

// Verify a Razorpay payment (client-side flow)
/**
 * @swagger
 * /razorpay/verify:
 *   post:
 *     summary: Verify a Razorpay payment signature
 *     tags:
 *       - Razorpay
 *     requestBody:
 *       required: true
 *       content:
 *         application/json:
 *           schema:
 *             type: object
 *             properties:
 *               razorpay_order_id:
 *                 type: string
 *               razorpay_payment_id:
 *                 type: string
 *               razorpay_signature:
 *                 type: string
 *             required:
 *               - razorpay_order_id
 *               - razorpay_payment_id
 *               - razorpay_signature
 *     responses:
 *       200:
 *         description: Verified
 *       400:
 *         description: Invalid signature or bad request
 *       500:
 *         description: Server error
 */
app.post('/razorpay/verify', async (req, res) => {
    const { razorpay_order_id, razorpay_payment_id, razorpay_signature } = req.body;
    if (!razorpay_order_id || !razorpay_payment_id || !razorpay_signature) {
        return res.status(400).json({ error: 'missing razorpay parameters' });
    }

    try {
        const verified = Payment.verifyRazorpaySignature({ razorpay_order_id, razorpay_payment_id, razorpay_signature });
        if (!verified) return res.status(400).json({ verified: false });

        Payment.update({status: 'completed'}, {
            where: { transactionId: razorpay_order_id, paymentMethod: 'razorpay' }
        }).then(() => {
            console.log(`Payment record with transactionId ${razorpay_order_id} marked as completed.`);
        }).catch((err) => {
            console.error('Failed to update payment record status:', err);
        });

        res.json({ verified: true });
    } catch (error) {
        console.error('Error verifying razorpay payment:', error);
        res.status(500).json({ error: error.message });
    }
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, async () => {
    try{
        console.log(`Server is running on port ${PORT}`);
        await initializeModels();
        await syncDatabase();
    } catch (error) {
        console.error("Error starting the server:", error);
    }
});

