import express from 'express';
import cors from 'cors';
import path from 'path';
import { fileURLToPath } from 'url';
import router from './routes/index.js';
import { swaggerSpec, swaggerMiddleware } from './config/swagger.js';
import { notFound, errorHandler } from './middlewares/errorHandler.js';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

export const createApp = () => {
  const app = express();

  app.use(express.json());
  app.use(cors({ origin: '*', methods: ['GET', 'POST', 'PUT', 'DELETE'] }));

  // public static files (e.g., Razorpay demo page)
  app.use(express.static(path.join(__dirname, '..', 'public')));

  // Swagger
  app.use('/api-docs', ...swaggerMiddleware);
  // quick root
  app.get('/', (req, res) => res.send('Payment Service is up'));

  // routes
  app.use('/', router);

  // fallthrough handlers
  app.use(notFound);
  app.use(errorHandler);

  return app;
};

export { swaggerSpec };

export default createApp;
