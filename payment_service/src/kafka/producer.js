import { getProducer } from './client.js';
import logger from '../utils/logger.js';

let producer;

export const initProducer = async () => {
  producer = getProducer();
  await producer.connect();
  return producer;
};

export const sendEvent = async (topic, payload, key) => {
  if (!producer) producer = getProducer();
  if (!producer) throw new Error('Kafka producer not initialized');
  try {
    await producer.send({ topic, messages: [{ key: key ?? null, value: JSON.stringify(payload) }] });
  } catch (err) {
    logger.error('Failed to send Kafka event', topic, err);
    throw err;
  }
};

export default { initProducer, sendEvent };
