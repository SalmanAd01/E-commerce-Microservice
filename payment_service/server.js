import express from 'express';
import { initializeModels, syncDatabase } from './models/index.js';
import Payment from './models/payment.js';
import swaggerUi from 'swagger-ui-express';
import swaggerJSDoc from 'swagger-jsdoc';
import cors from 'cors';
import { Kafka, Partitioners } from 'kafkajs';
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
let kafkaProducer = null;

app.post('/razorpay/verify', async (req, res) => {
    const { razorpay_order_id, razorpay_payment_id, razorpay_signature } = req.body;
    if (!razorpay_order_id || !razorpay_payment_id || !razorpay_signature) {
        return res.status(400).json({ error: 'missing razorpay parameters' });
    }

    try {
        const verified = Payment.verifyRazorpaySignature({ razorpay_order_id, razorpay_payment_id, razorpay_signature });
        if (!verified) {
            // mark payment as failed in DB (if exists) and publish payment.failed
            try {
                await Payment.update({ status: 'failed' }, { where: { transactionId: razorpay_order_id, paymentMethod: 'razorpay' } });
                const paymentRecord = await Payment.findOne({ where: { transactionId: razorpay_order_id } });
                const orderId = paymentRecord ? paymentRecord.orderId : null;
                if (kafkaProducer) {
                    const outEvent = { orderId: orderId, transactionId: razorpay_order_id, status: 'failed', reason: 'verification_failed' };
                    await kafkaProducer.send({ topic: 'payment.failed', messages: [{ key: String(orderId), value: JSON.stringify(outEvent) }] });
                }
            } catch (err) {
                console.error('Failed to mark payment failed during verification:', err);
            }
            return res.status(400).json({ verified: false });
        }
        // mark payment as completed in DB and publish payment.succeeded event
        try {
            const [count] = await Payment.update({ status: 'completed' }, { where: { transactionId: razorpay_order_id, paymentMethod: 'razorpay' } });
            if (count === 0) {
                console.warn('No payment record found to update for transaction', razorpay_order_id);
            }

            // fetch the payment record to get orderId
            const paymentRecord = await Payment.findOne({ where: { transactionId: razorpay_order_id } });
            const orderId = paymentRecord ? paymentRecord.orderId : null;

            // publish payment.succeeded
            if (kafkaProducer) {
                const outEvent = { orderId: orderId, transactionId: razorpay_order_id, status: 'succeeded' };
                await kafkaProducer.send({ topic: 'payment.succeeded', messages: [{ key: String(orderId), value: JSON.stringify(outEvent) }] });
                console.log('Published payment.succeeded for order', orderId);
            } else {
                console.warn('Kafka producer not available, skipping publishing payment.succeeded');
            }

            res.json({ verified: true });
        } catch (err) {
            console.error('Failed to update payment record status or publish event:', err);
            return res.status(500).json({ error: 'failed to complete payment post-verification' });
        }
    } catch (error) {
        console.error('Error verifying razorpay payment:', error);
        res.status(500).json({ error: error.message });
    }
});

const PORT = process.env.PORT || 3000;
async function start() {
    // Kafka setup
    const kafka = new Kafka({ brokers: ['localhost:7092'], clientId: 'payment-service' });

    // Use legacy partitioner to retain prior partitioning behavior and silence warning
    const producer = kafka.producer({ createPartitioner: Partitioners.LegacyPartitioner });
    const consumer = kafka.consumer({ groupId: 'payment-service-group' });

    // Ensure topics exist using admin API before connecting consumer/producer
    const admin = kafka.admin();
    await admin.connect();
    const topicsToEnsure = [
        'inventory.reserved', 'inventory.reservation_failed', 'inventory.released',
        'payment.initiated', 'payment.succeeded', 'payment.failed'
    ];
    try {
        await admin.createTopics({
            topics: topicsToEnsure.map(t => ({ topic: t, numPartitions: 1, replicationFactor: 1 })),
            waitForLeaders: true
        });
        console.log('Ensured Kafka topics:', topicsToEnsure.join(', '));
    } catch (err) {
        console.warn('Could not ensure Kafka topics (they may already exist or broker may not allow creation):', err.message || err);
    }
    await admin.disconnect();

    await producer.connect();
    await consumer.connect();
    kafkaProducer = producer;

    // Listen for inventory.reserved events
    await consumer.subscribe({ topic: 'inventory.reserved', fromBeginning: true });
    consumer.run({
        eachMessage: async ({ topic, partition, message }) => {
            try {
                const payload = message.value.toString();
                console.log('Payment service received event on', topic, payload);

                let event = {};
                try { event = JSON.parse(payload); } catch (e) { }

                // Production behavior: create a payment record with status 'pending' and let
                // the client/payment gateway complete the payment and call /razorpay/verify.
                // Do NOT simulate success/failure here.
                const orderId = event.orderId || null;
                const amount = event.amount || null;
                const currency = event.currency || 'INR';
                
                const order = await Payment.createRazorpayOrder({
                    amount: amount,
                    currency: currency,
                    receipt: `autopay_rcpt_${orderId || 'unknown'}`,
                    payment_capture: 1
                });
                // create a DB payment record linking to the local orderId
                try {
                    await Payment.createPaymentRecord({
                        orderId: orderId,
                        paymentMethod: 'autopay',
                        transactionId: order.id,
                        amount: amount || 0,
                        currency: currency,
                        status: 'pending'
                    });

                    // Publish an event indicating payment was initiated; downstream systems
                    // can observe this and the client will still need to complete payment.
                    const initiatedEvent = { orderId: orderId, transactionId: order.id, status: 'pending' };
                    await producer.send({ topic: 'payment.initiated', messages: [{ key: String(orderId), value: JSON.stringify(initiatedEvent) }] });
                    console.log('Payment initiated for order', orderId, initiatedEvent);
                } catch (dbErr) {
                    console.error('Failed to create payment record:', dbErr);
                    // publish payment.failed so saga can react to failure to start payment
                    const failedEvent = { orderId: orderId, status: 'failed', reason: 'db_error' };
                    await producer.send({ topic: 'payment.failed', messages: [{ key: String(orderId), value: JSON.stringify(failedEvent) }] });
                }
            } catch (err) {
                console.error('Error handling inventory.reserved message', err);
            }
        }
    });

    try{
        console.log(`Server is running on port ${PORT}`);
        await initializeModels();
        await syncDatabase();
    } catch (error) {
        console.error("Error starting the server:", error);
    }

    app.listen(PORT, () => console.log(`HTTP server listening on ${PORT}`));
}

start().catch(err => console.error('Failed to start service', err));

