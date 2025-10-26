import { getKafka } from './client.js';
import config from '../config/env.js';
import logger from '../utils/logger.js';

let consumerInstance;

export const getConsumer = () => {
  if (!consumerInstance) consumerInstance = getKafka().consumer({ groupId: config.kafka.groupId });
  return consumerInstance;
};

export const startConsumer = async (subscriptions = [], handler) => {
  const consumer = getConsumer();
  await consumer.connect();
  for (const topic of subscriptions) {
    await consumer.subscribe({ topic, fromBeginning: true });
  }
  await consumer.run({
    eachMessage: async ({ topic, partition, message }) => { // eslint-disable-line no-unused-vars
      try {
        const payloadStr = message.value.toString();
        logger.info('Received event', topic, payloadStr);
        let event = {};
        try { event = JSON.parse(payloadStr); } catch (e) { /* ignore */ }
        await handler(topic, event);
      } catch (err) {
        logger.error('Error handling message', err);
      }
    },
  });
  return consumer;
};

export default { getConsumer, startConsumer };
