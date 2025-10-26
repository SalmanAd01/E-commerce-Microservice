import { PaymentService } from '../../services/payment.service.js';
import { TOPICS } from '../topics.js';

export const inventoryReservedHandler = async (topic, event) => {
  if (topic !== TOPICS.INVENTORY_RESERVED) return;
  await PaymentService.handleInventoryReserved(event);
};

export default inventoryReservedHandler;
