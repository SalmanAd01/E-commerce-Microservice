# Payment Service

A Node.js payment microservice for an e-commerce system. It exposes REST APIs to create Razorpay orders and verify payments, persists payment records in Postgres via Sequelize, and integrates with Kafka for saga coordination.

## Features
- Express 5 app with layered architecture (routes → controllers → services → providers)
- Sequelize/Postgres `Payment` model
- Razorpay provider abstraction with a pluggable factory
- Kafka integration (initiate/emit payment events, handle `inventory.reserved`)
- Centralized config, logging, and error handling
- Swagger docs at `/api-docs`
- Static demo page under `public/razorpay_checkout.html`

## Configuration
Copy `.env.example` to `.env` and fill in values:

- `DATABASE_URL` (Postgres)
- `RAZORPAY_KEY_ID`, `RAZORPAY_KEY_SECRET`
- `KAFKA_BROKERS` (comma-separated if multiple)

## Run

Development (with env file and watch):

```sh
npm run dev
```

Production:

```sh
npm start
```

The service listens on `PORT` (default 8000). API docs at `http://localhost:8000/api-docs`.

## API
- POST `/razorpay/order` → create provider order; optionally persists pending record if `localOrderId` is provided.
- POST `/razorpay/verify` → verify signature; updates DB and emits Kafka events.

## Kafka
- Consumes: `inventory.reserved`
- Produces: `payment.initiated`, `payment.succeeded`, `payment.failed`

## Project Layout
```
src/
  app.js                  # Express assembly
  config/                 # env + swagger
  controllers/            # HTTP controllers
  kafka/                  # client, producer, consumer, topics, handlers
  middlewares/            # error handling
  routes/                 # route definitions
  services/               # business logic and providers
  utils/                  # logger, async handler
models/                   # Sequelize models (Payment)
public/                   # Static demo assets
```

## Notes
- This refactor keeps Sequelize models in `models/` and moves app logic to `src/` for clarity.
- The provider pattern allows adding more gateways without touching controllers/services significantly.
- Replace the lightweight logger with pino/winston if you prefer structured logs.
