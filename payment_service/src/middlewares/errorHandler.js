import logger from '../utils/logger.js';

export class ApiError extends Error {
  constructor(statusCode, message, details) {
    super(message);
    this.statusCode = statusCode;
    this.details = details;
  }
}

export const notFound = (req, res, next) => {
  next(new ApiError(404, `Route ${req.originalUrl} not found`));
};

export const errorHandler = (err, req, res, next) => { // eslint-disable-line no-unused-vars
  const status = err.statusCode || 500;
  const payload = {
    error: err.message || 'Internal Server Error',
  };
  if (err.details) payload.details = err.details;
  if (process.env.DEBUG === 'true') payload.stack = err.stack;

  if (status >= 500) logger.error(err);
  else logger.warn(`Handled error ${status}:`, err.message);

  res.status(status).json(payload);
};

export default errorHandler;
