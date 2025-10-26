import Payment from '../../models/payment.js';
import { PaymentProviderFactory } from './providers/providerFactory.js';
import { sendEvent } from '../kafka/producer.js';
import { TOPICS } from '../kafka/topics.js';
import logger from '../utils/logger.js';

let providerInstance;
const getProvider = () => {
  if (!providerInstance) providerInstance = PaymentProviderFactory('razorpay');
  return providerInstance;
};

export class PaymentService {
  // Create a provider order and optionally persist a pending record linked to local order
  static async createOrder({ amount, currency = 'INR', receipt, payment_capture = 1, localOrderId }) {
  const order = await getProvider().createOrder({ amount, currency, receipt, payment_capture });

    if (localOrderId != null) {
      await Payment.createPaymentRecord({
        orderId: localOrderId,
        paymentMethod: 'razorpay',
        transactionId: order.id,
        amount: amount || (order.amount ? Number(order.amount) / 100 : 0),
        currency: currency || order.currency || 'INR',
        status: 'pending',
      });
    }
    return order;
  }

  // Verify signature, update DB, and emit Kafka events
  static async verifySignatureAndFinalize({ razorpay_order_id, razorpay_payment_id, razorpay_signature }) {
  const verified = getProvider().verifySignature({ razorpay_order_id, razorpay_payment_id, razorpay_signature });
    if (!verified) {
      // mark failed if record exists and push event
      try {
        await Payment.update({ status: 'failed' }, { where: { transactionId: razorpay_order_id, paymentMethod: 'razorpay' } });
        const paymentRecord = await Payment.findOne({ where: { transactionId: razorpay_order_id } });
        const orderId = paymentRecord ? paymentRecord.orderId : null;
        if (orderId != null) {
          await sendEvent(TOPICS.PAYMENT_FAILED, { orderId, transactionId: razorpay_order_id, status: 'failed', reason: 'verification_failed' }, String(orderId));
        }
      } catch (err) {
        logger.error('Failed to mark payment failed during verification:', err);
      }
      return { verified: false };
    }

    // success: mark completed and emit
    const [count] = await Payment.update(
      { status: 'completed' },
      { where: { transactionId: razorpay_order_id, paymentMethod: 'razorpay' } }
    );
    if (count === 0) logger.warn('No payment record found to update for transaction', razorpay_order_id);

    const paymentRecord = await Payment.findOne({ where: { transactionId: razorpay_order_id } });
    const orderId = paymentRecord ? paymentRecord.orderId : null;
    if (orderId != null) {
      await sendEvent(TOPICS.PAYMENT_SUCCEEDED, { orderId, transactionId: razorpay_order_id, status: 'succeeded' }, String(orderId));
      logger.info('Published payment.succeeded for order', orderId);
    } else {
      logger.warn('No orderId found for transaction', razorpay_order_id);
    }

    return { verified: true };
  }

  // Saga handler for inventory.reserved
  static async handleInventoryReserved(event) {
    const orderId = event.orderId ?? null;
    const amount = event.amount ?? null;
    const currency = event.currency || 'INR';

    try {
  const order = await getProvider().createOrder({
        amount,
        currency,
        receipt: `autopay_rcpt_${orderId || 'unknown'}`,
        payment_capture: 1,
      });

      // persist pending payment
      await Payment.createPaymentRecord({
        orderId,
        paymentMethod: 'razorpay',
        transactionId: order.id,
        amount: amount || 0,
        currency,
        status: 'pending',
      });

      // notify payment initiated
      await sendEvent(TOPICS.PAYMENT_INITIATED, { orderId, transactionId: order.id, status: 'pending' }, String(orderId));
      logger.info('Payment initiated for order', orderId, { transactionId: order.id });
    } catch (err) {
      logger.error('Failed to initiate payment for order', orderId, err);
      await sendEvent(TOPICS.PAYMENT_FAILED, { orderId, status: 'failed', reason: 'db_or_provider_error' }, String(orderId));
    }
  }
}

export default PaymentService;
