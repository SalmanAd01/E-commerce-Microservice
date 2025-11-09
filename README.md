# E-commerce Microservices

This repository contains a polyglot e‑commerce system composed of multiple services:
- Order Service (Java, gRPC/Kafka)
- Inventory Service (.NET)
- Payment Service (Node.js)
- Shared messaging via Kafka

## Order Saga Orchestration (Happy path, failures, and compensations)

The sequence below shows the end‑to‑end saga for placing an order, reserving inventory, processing payment, and either committing inventory or compensating on failure.

```mermaid
sequenceDiagram
	autonumber
	participant C as Client
	participant GW as Gateway
	participant OS as Order Service (Java)
	participant IS as Inventory Service (.NET)
	participant PS as Payment Service (Node.js)
	participant K as Kafka

	C->>GW: POST /orders
	GW->>OS: Create Order
	OS-->>GW: Return selling price (order_status: PENDING)
	OS->>K: ordercreated
	K-->>IS: ordercreated

	alt Inventory Reservation Successful
		IS->>K: inventory_reserved
		K-->>OS: inventory_reserved
		OS->>K: payment_initiated
		K-->>PS: payment_initiated
	else Inventory Reservation Failed
		IS->>K: inventory_reservation_failed
		K-->>OS: inventory_reservation_failed
		OS->>K: order_cancelled
		K-->>IS: order_cancelled
		IS->>K: inventory_released (compensation)
		K-->>OS: inventory_released
		OS-->>GW: order_status: FAILED
		GW-->>C: Failure response
	end

	C->>GW: POST /payment/verify
	GW->>PS: POST /verify

	alt Payment Succeeded
		PS->>K: payment_succeeded
		K-->>OS: payment_succeeded
		GW->>OS: POST /order/complete
		OS->>K: order_completed
		K-->>IS: order_completed
		alt Inventory Commit Successful
			IS->>K: inventory_committed
			K-->>OS: inventory_committed (order_status: COMPLETED)
		else Inventory Commit Failed
			IS->>K: inventory_commit_failed
			K-->>OS: inventory_commit_failed
			OS->>K: order_cancelled
			K-->>IS: order_cancelled
			IS->>K: inventory_released (compensation)
			K-->>OS: inventory_released
		end
	else Payment Failed
		PS->>K: payment_failed
		K-->>OS: payment_failed
		OS->>K: order_cancelled
		K-->>IS: order_cancelled
		IS->>K: inventory_released (compensation)
		K-->>OS: inventory_released
	end
```

Notes
- All service-to-service events are published/consumed via Kafka.
- Order statuses progress through PENDING → PAYMENT_INITIATED → COMPLETED or FAILED.
- Inventory uses a reservation pattern with commit on success and release on failure.

