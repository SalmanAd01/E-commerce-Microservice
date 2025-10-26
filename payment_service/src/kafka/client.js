import { Kafka, Partitioners } from 'kafkajs';
import config from '../config/env.js';
import logger from '../utils/logger.js';
import { TOPICS } from './topics.js';

let kafkaInstance;
let adminInstance;
let producerInstance;

export const getKafka = () => {
  if (!kafkaInstance) {
    kafkaInstance = new Kafka({
      clientId: config.kafka.clientId,
      brokers: config.kafka.brokers,
    });
  }
  return kafkaInstance;
};

export const getAdmin = () => {
  if (!adminInstance) adminInstance = getKafka().admin();
  return adminInstance;
};

export const getProducer = () => {
  if (!producerInstance) producerInstance = getKafka().producer({ createPartitioner: Partitioners.LegacyPartitioner });
  return producerInstance;
};

export const ensureTopics = async () => {
  const admin = getAdmin();
  await admin.connect();
  const topicsToEnsure = Object.values(TOPICS);
  try {
    await admin.createTopics({
      topics: topicsToEnsure.map((t) => ({ topic: t, numPartitions: 1, replicationFactor: 1 })),
      waitForLeaders: true,
    });
    logger.info('Ensured Kafka topics:', topicsToEnsure.join(', '));
  } catch (err) {
    logger.warn('Could not ensure Kafka topics (maybe existing):', err.message || err);
  }
  await admin.disconnect();
};
