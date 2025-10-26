import { PaymentService } from '../services/payment.service.js';
import asyncHandler from '../utils/asyncHandler.js';
import { ApiError } from '../middlewares/errorHandler.js';

export const createRazorpayOrder = asyncHandler(async (req, res) => {
  const { amount, currency, receipt, payment_capture, localOrderId } = req.body || {};
  if (amount == null) throw new ApiError(400, 'amount is required');
  const order = await PaymentService.createOrder({ amount, currency, receipt, payment_capture, localOrderId });
  res.json(order);
});

export const verifyRazorpayPayment = asyncHandler(async (req, res) => {
  const { razorpay_order_id, razorpay_payment_id, razorpay_signature } = req.body || {};
  if (!razorpay_order_id || !razorpay_payment_id || !razorpay_signature) {
    throw new ApiError(400, 'missing razorpay parameters');
  }
  const result = await PaymentService.verifySignatureAndFinalize({ razorpay_order_id, razorpay_payment_id, razorpay_signature });
  if (!result.verified) return res.status(400).json(result);
  res.json(result);
});

export default { createRazorpayOrder, verifyRazorpayPayment };
