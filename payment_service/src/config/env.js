// Centralized environment and configuration handling

const toArray = (val, def = []) =>
  (val && typeof val === 'string') ? val.split(',').map(s => s.trim()).filter(Boolean) : def;

export const config = {
  serviceName: process.env.SERVICE_NAME || 'payment-service',
  nodeEnv: process.env.NODE_ENV || 'development',
  isProd: (process.env.NODE_ENV || 'development') === 'production',
  port: Number(process.env.PORT) || 8000,
  dbUrl: process.env.DATABASE_URL,

  razorpay: {
    keyId: process.env.RAZORPAY_KEY_ID || '',
    keySecret: process.env.RAZORPAY_KEY_SECRET || '',
  },

  kafka: {
    clientId: process.env.KAFKA_CLIENT_ID || 'payment-service',
    brokers: toArray(process.env.KAFKA_BROKERS, ['localhost:7092']),
    groupId: process.env.KAFKA_GROUP_ID || 'payment-service-group',
  },

  swagger: {
    serverUrl: process.env.SWAGGER_SERVER_URL || `http://localhost:${Number(process.env.PORT) || 8000}`,
  },
};

export default config;
