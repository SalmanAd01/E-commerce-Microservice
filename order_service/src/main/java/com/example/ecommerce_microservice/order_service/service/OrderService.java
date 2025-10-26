package com.example.ecommerce_microservice.order_service.service;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;

import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.http.HttpStatusCode;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.server.ResponseStatusException;

import com.example.ecommerce_microservice.order_service.dto.CreateOrderDto;
import com.example.ecommerce_microservice.order_service.dto.CreateOrderItemDto;
import com.example.ecommerce_microservice.order_service.entity.Order;
import com.example.ecommerce_microservice.order_service.entity.OrderItem;
import com.example.ecommerce_microservice.order_service.repository.OrderRepository;

@Service
public class OrderService {

    private final OrderRepository orderRepository;
    private final KafkaTemplate<String, String> kafkaTemplate;
    private final RestTemplate restTemplate = new RestTemplate();

    public OrderService(OrderRepository orderRepository, KafkaTemplate<String, String> kafkaTemplate) {
        this.orderRepository = orderRepository;
        this.kafkaTemplate = kafkaTemplate;
    }

    /**
     * Create an order using the provided DTO. For each item, validate/update the price
     * using the inventory service's sellingPrice.
     */
    public Order createOrder(CreateOrderDto createOrderDto) {
        Long customerId = createOrderDto.getCustomerId();

        Order order = Order.builder()
                .customerId(customerId)
                .status(Order.OrderStatus.PENDING)
                .createdAt(LocalDateTime.now())
                .updatedAt(LocalDateTime.now())
                .build();

        List<OrderItem> orderItems = new ArrayList<>();
        if (createOrderDto.getItems() != null) {
            for (CreateOrderItemDto it : createOrderDto.getItems()) {
                // fetch price from inventory service and set it on the order item
                Double priceToUse = 0.0;
                String url = String.format("http://localhost:5168/api/v1/inventories/store/%d/productsku/%s",
                        it.getStoreId(), it.getProductSku());
                try {
                    ResponseEntity<Map<String, Object>> respEntity = restTemplate.exchange(
                            url,
                            HttpMethod.GET,
                            null,
                            new ParameterizedTypeReference<Map<String, Object>>() {
                            });
                    HttpStatusCode status = respEntity.getStatusCode();
                    if (status != null && status.is2xxSuccessful()) {
                        Map<String, Object> resp = respEntity.getBody();
                        if (resp != null && resp.containsKey("sellingPrice")) {
                            Object sp = resp.get("sellingPrice");
                            if (sp != null) {
                                priceToUse = sp instanceof Number ? ((Number) sp).doubleValue()
                                        : Double.valueOf(sp.toString());
                            }
                        }
                    } else {
                        throw new ResponseStatusException(HttpStatusCode.valueOf(503),
                                "Inventory service returned status: " + (status == null ? "null" : status.value()));
                    }
                } catch (ResponseStatusException rse) {
                    throw rse;
                } catch (Exception ex) {
                    throw new ResponseStatusException(HttpStatusCode.valueOf(503),
                            "Inventory validation failed: " + ex.getMessage(), ex);
                }

                OrderItem oi = OrderItem.builder()
                        .order(order)
                        .productSku(it.getProductSku())
                        .storeId(it.getStoreId())
                        .quantity(it.getQuantity())
                        .price(priceToUse)
                        .createdAt(LocalDateTime.now())
                        .updatedAt(LocalDateTime.now())
                        .build();
                // set totalPrice explicitly
                oi.setTotalPrice(oi.getPrice() * oi.getQuantity());
                orderItems.add(oi);
            }
        }

        if (!orderItems.isEmpty()) {
            order.setOrderItems(orderItems);
            // compute total amount from items
            double total = orderItems.stream().mapToDouble(i -> i.getTotalPrice() == null ? 0.0 : i.getTotalPrice()).sum();
            order.setTotalAmount(total);
        }

        Order saved = orderRepository.save(order);

        // Publish OrderCreated event as JSON including items
        try {
            Map<String, Object> payload = Map.of(
                    "orderId", saved.getId(),
                    "customerId", saved.getCustomerId(),
                    "totalAmount", saved.getTotalAmount(),
                    "status", saved.getStatus().name(),
                    "createdAt", saved.getCreatedAt().toString(),
                    "items", saved.getOrderItems().stream().map(oi -> Map.of(
                            "productSku", oi.getProductSku(),
                            "storeId", oi.getStoreId(),
                            "quantity", oi.getQuantity(),
                            "price", oi.getPrice(),
                            "totalPrice", oi.getTotalPrice()
                    )).toList()
            );
            String event = new com.fasterxml.jackson.databind.ObjectMapper().writeValueAsString(payload);
            kafkaTemplate.send("order.created", event);
        } catch (Exception ex) {
            System.err.println("Failed to publish order.created: " + ex.getMessage());
        }

        return saved;
    }
    
        /**
         * Mark order as COMPLETED only if current status is PAYMENT_SUCCESSFUL.
         * Publishes an "order.completed" event with full order details including items.
         */
        public Order completeOrder(Long orderId) {
            var order = orderRepository.findById(orderId)
                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Order not found"));

            if (order.getStatus() != Order.OrderStatus.PAYMENT_SUCCESSFUL) {
                throw new ResponseStatusException(HttpStatus.BAD_REQUEST,
                        "Order status must be PAYMENT_SUCCESSFUL to complete");
            }

            order.setStatus(Order.OrderStatus.COMPLETED);
            order.setUpdatedAt(LocalDateTime.now());
            Order saved = orderRepository.save(order);

            // Publish OrderCompleted event as JSON including items
            try {
                var payload = new java.util.HashMap<String, Object>();
                payload.put("orderId", saved.getId());
                payload.put("customerId", saved.getCustomerId());
                payload.put("amount", saved.getTotalAmount());
                payload.put("status", saved.getStatus() == null ? null : saved.getStatus().name());
                payload.put("createdAt", saved.getCreatedAt().toString());
                payload.put("updatedAt", saved.getUpdatedAt().toString());
                java.util.List<java.util.Map<String, Object>> evItems = new java.util.ArrayList<>();
                if (saved.getOrderItems() != null) {
                    for (var oi : saved.getOrderItems()) {
                        var m = new java.util.HashMap<String, Object>();
                        m.put("id", oi.getId());
                        m.put("storeId", oi.getStoreId());
                        m.put("productSku", oi.getProductSku());
                        m.put("quantity", oi.getQuantity());
                        m.put("price", oi.getPrice());
                        m.put("totalPrice", oi.getTotalPrice());
                        evItems.add(m);
                    }
                }
                payload.put("items", evItems);
                String event = new com.fasterxml.jackson.databind.ObjectMapper().writeValueAsString(payload);
                kafkaTemplate.send("order.completed", event);
            } catch (Exception ex) {
                System.err.println("Failed to publish order.completed: " + ex.getMessage());
            }

            return saved;
        }

    public Order getOrderById(Long orderId) {
        return orderRepository.findById(orderId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Order not found"));

    }

    public List<Order> getAllOrders() {
        return orderRepository.findAll();
    }

    public void cancelOrder(Long orderId) {
        var order = orderRepository.findById(orderId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Order not found"));

        order.setStatus(Order.OrderStatus.CANCELLED);
        order.setUpdatedAt(LocalDateTime.now());
        Order saved = orderRepository.save(order);

        // Publish OrderCancelled event
        try {
                var payload = new java.util.HashMap<String, Object>();
                payload.put("orderId", saved.getId());
                payload.put("customerId", saved.getCustomerId());
                payload.put("amount", saved.getTotalAmount());
                payload.put("status", saved.getStatus() == null ? null : saved.getStatus().name());
                payload.put("createdAt", saved.getCreatedAt().toString());
                payload.put("updatedAt", saved.getUpdatedAt().toString());
                java.util.List<java.util.Map<String, Object>> evItems = new java.util.ArrayList<>();
                if (saved.getOrderItems() != null) {
                    for (var oi : saved.getOrderItems()) {
                        var m = new java.util.HashMap<String, Object>();
                        m.put("id", oi.getId());
                        m.put("storeId", oi.getStoreId());
                        m.put("productSku", oi.getProductSku());
                        m.put("quantity", oi.getQuantity());
                        m.put("price", oi.getPrice());
                        m.put("totalPrice", oi.getTotalPrice());
                        evItems.add(m);
                    }
                }
                payload.put("items", evItems);
            String event = new com.fasterxml.jackson.databind.ObjectMapper().writeValueAsString(payload);
            kafkaTemplate.send("order.cancelled", event);
        } catch (Exception ex) {
            System.err.println("Failed to publish order.cancelled: " + ex.getMessage());
        }
    }
}
