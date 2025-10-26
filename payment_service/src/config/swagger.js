import swaggerUi from 'swagger-ui-express';
import swaggerJSDoc from 'swagger-jsdoc';
import config from './env.js';

const swaggerOptions = {
  definition: {
    openapi: '3.0.0',
    info: {
      title: 'Payment Service API',
      version: '1.0.0',
      description: 'APIs for creating Razorpay orders and verifying payments',
    },
    servers: [{ url: config.swagger.serverUrl }],
  },
  apis: ['./src/routes/*.js'],
};

export const swaggerSpec = swaggerJSDoc(swaggerOptions);
export const swaggerMiddleware = [swaggerUi.serve, swaggerUi.setup(swaggerSpec)];

export default swaggerMiddleware;
