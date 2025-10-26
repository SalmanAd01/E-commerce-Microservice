import { Router } from 'express';
import { createRazorpayOrder, verifyRazorpayPayment } from '../controllers/payment.controller.js';

const router = Router();

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
router.post('/razorpay/order', createRazorpayOrder);

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
router.post('/razorpay/verify', verifyRazorpayPayment);

export default router;
