import Razorpay from 'razorpay';
import crypto from 'crypto';
import config from '../../config/env.js';

export class RazorpayProvider {
  constructor() {
    if (!config.razorpay.keyId || !config.razorpay.keySecret) {
      throw new Error('RAZORPAY_KEY_ID and RAZORPAY_KEY_SECRET must be set');
    }
    this.client = new Razorpay({ key_id: config.razorpay.keyId, key_secret: config.razorpay.keySecret });
  }

  async createOrder({ amount, currency = 'INR', receipt, payment_capture = 1 }) {
    const opts = {
      amount: Math.round(Number(amount) * 100),
      currency,
      receipt: receipt || `rcpt_${Date.now()}`,
      payment_capture,
    };
    return this.client.orders.create(opts);
  }

  verifySignature({ razorpay_order_id, razorpay_payment_id, razorpay_signature }) {
    const generated = crypto
      .createHmac('sha256', config.razorpay.keySecret)
      .update(`${razorpay_order_id}|${razorpay_payment_id}`)
      .digest('hex');
    return generated === razorpay_signature;
  }
}

export default RazorpayProvider;
