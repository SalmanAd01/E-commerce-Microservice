package com.example.ecommerce_microservice.order_service.events;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Component;

import com.example.ecommerce_microservice.order_service.entity.Order;
import com.example.ecommerce_microservice.order_service.entity.OrderItem;
import com.fasterxml.jackson.databind.ObjectMapper;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
@RequiredArgsConstructor
public class OrderEventPublisher {

    private final KafkaTemplate<String, String> kafkaTemplate;
    private final ObjectMapper objectMapper;

    @Value("${app.kafka.topics.order-created:order.created}")
    private String orderCreatedTopic;
    @Value("${app.kafka.topics.order-completed:order.completed}")
    private String orderCompletedTopic;
    @Value("${app.kafka.topics.order-cancelled:order.cancelled}")
    private String orderCancelledTopic;

    public void publishOrderCreated(Order order) {
        publish(orderCreatedTopic, buildOrderPayload(order));
    }

    public void publishOrderCompleted(Order order) {
        publish(orderCompletedTopic, buildOrderPayload(order));
    }

    public void publishOrderCancelled(Order order, String reason) {
        Map<String, Object> payload = buildOrderPayload(order);
        if (reason != null) payload.put("reason", reason);
        publish(orderCancelledTopic, payload);
    }

    private void publish(String topic, Map<String, Object> payload) {
        try {
            String event = objectMapper.writeValueAsString(payload);
            kafkaTemplate.send(topic, event);
        } catch (Exception ex) {
            log.error("Failed to publish {}: {}", topic, ex.getMessage());
        }
    }

    private Map<String, Object> buildOrderPayload(Order saved) {
        Map<String, Object> payload = new HashMap<>();
        payload.put("orderId", saved.getId());
        payload.put("customerId", saved.getCustomerId());
        payload.put("totalAmount", saved.getTotalAmount());
        payload.put("status", saved.getStatus() == null ? null : saved.getStatus().name());
        payload.put("createdAt", saved.getCreatedAt() == null ? null : saved.getCreatedAt().toString());
        payload.put("updatedAt", saved.getUpdatedAt() == null ? null : saved.getUpdatedAt().toString());
        List<Map<String, Object>> items = saved.getOrderItems() == null ? List.of()
                : saved.getOrderItems().stream().map(this::toItem).collect(Collectors.toList());
        payload.put("items", items);
        return payload;
    }

    private Map<String, Object> toItem(OrderItem oi) {
        Map<String, Object> m = new HashMap<>();
        m.put("id", oi.getId());
        m.put("storeId", oi.getStoreId());
        m.put("productSku", oi.getProductSku());
        m.put("quantity", oi.getQuantity());
        m.put("price", oi.getPrice());
        m.put("totalPrice", oi.getTotalPrice());
        return m;
    }
}
