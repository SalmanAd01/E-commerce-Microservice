import { initializeModels, syncDatabase } from './models/index.js';
import createApp from './src/app.js';
import config from './src/config/env.js';
import logger from './src/utils/logger.js';
import { ensureTopics } from './src/kafka/client.js';
import { initProducer } from './src/kafka/producer.js';
import { startConsumer } from './src/kafka/consumer.js';
import { TOPICS } from './src/kafka/topics.js';
import inventoryReservedHandler from './src/kafka/handlers/inventoryReserved.handler.js';

const app = createApp();

async function start() {
  try {
    await initializeModels();
    await syncDatabase();

    // Kafka setup
    await ensureTopics();
    await initProducer();
    await startConsumer([TOPICS.INVENTORY_RESERVED], inventoryReservedHandler);

    app.listen(config.port, () => logger.info(`HTTP server listening on ${config.port}`));
  } catch (error) {
    logger.error('Error starting the server:', error);
    process.exitCode = 1;
  }
}

start().catch((err) => logger.error('Failed to start service', err));

