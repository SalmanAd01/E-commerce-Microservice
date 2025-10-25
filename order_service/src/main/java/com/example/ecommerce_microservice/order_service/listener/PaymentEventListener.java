package com.example.ecommerce_microservice.order_service.listener;

import java.util.Map;

import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Component;

import com.example.ecommerce_microservice.order_service.entity.Order;
import com.example.ecommerce_microservice.order_service.repository.OrderRepository;
import com.fasterxml.jackson.databind.ObjectMapper;

@Component
public class PaymentEventListener {

    private final OrderRepository orderRepository;
    private final KafkaTemplate<String, String> kafkaTemplate;
    private final ObjectMapper objectMapper = new ObjectMapper();

    public PaymentEventListener(OrderRepository orderRepository, KafkaTemplate<String, String> kafkaTemplate) {
        this.orderRepository = orderRepository;
        this.kafkaTemplate = kafkaTemplate;
    }

    @KafkaListener(topics = {"payment.succeeded", "payment.failed"}, groupId = "order-service-group")
    public void onPaymentEvent(String message) {
        try {
            @SuppressWarnings("unchecked")
            Map<String, Object> event = objectMapper.readValue(message, Map.class);
            Long orderId = Long.valueOf(String.valueOf(event.get("orderId")));
            String status = (String) event.get("status");

            orderRepository.findById(orderId).ifPresent(order -> {
                if ("succeeded".equalsIgnoreCase(status)) {
                    order.setStatus(Order.OrderStatus.COMPLETED);
                } else {
                    order.setStatus(Order.OrderStatus.CANCELLED);
                    // publish order.cancelled event so other services (like inventory) can react
                    try {
                        Map<String, Object> out = Map.of("orderId", orderId, "reason", "payment_failed");
                        String outEvent = objectMapper.writeValueAsString(out);
                        kafkaTemplate.send("order.cancelled", outEvent);
                    } catch (Exception e) {
                        System.err.println("Failed to publish order.cancelled: " + e.getMessage());
                    }
                }
                orderRepository.save(order);
            });
        } catch (Exception ex) {
            System.err.println("Failed to handle payment event: " + ex.getMessage());
        }
    }
}
