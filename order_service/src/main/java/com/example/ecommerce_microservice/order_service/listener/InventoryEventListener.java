package com.example.ecommerce_microservice.order_service.listener;

import java.util.Map;

import org.springframework.kafka.annotation.KafkaListener;
import org.springframework.kafka.core.KafkaTemplate;
import org.springframework.stereotype.Component;

import com.example.ecommerce_microservice.order_service.entity.Order;
import com.example.ecommerce_microservice.order_service.repository.OrderRepository;
import com.fasterxml.jackson.databind.ObjectMapper;

@Component
public class InventoryEventListener {

    private final OrderRepository orderRepository;
    private final KafkaTemplate<String, String> kafkaTemplate;
    private final ObjectMapper objectMapper = new ObjectMapper();

    public InventoryEventListener(OrderRepository orderRepository, KafkaTemplate<String, String> kafkaTemplate) {
        this.orderRepository = orderRepository;
        this.kafkaTemplate = kafkaTemplate;
    }

    @KafkaListener(topics = {"inventory.reservation_failed"}, groupId = "order-service-group")
    public void onInventoryReservationFailed(String message) {
        try {
            @SuppressWarnings("unchecked")
            Map<String, Object> event = objectMapper.readValue(message, Map.class);
            if (event.get("orderId") != null) {
                Long parsedOrderId = Long.valueOf(String.valueOf(event.get("orderId")));
                final Long orderIdFinal = parsedOrderId;
                orderRepository.findById(orderIdFinal).ifPresent(order -> {
                    order.setStatus(Order.OrderStatus.CANCELLED);
                    orderRepository.save(order);
                    // publish order.cancelled for other services (inventory will react to releases if needed)
                    try {
                        Map<String, Object> out = Map.of("orderId", orderIdFinal, "reason", "inventory_reservation_failed");
                        String outEvent = objectMapper.writeValueAsString(out);
                        kafkaTemplate.send("order.cancelled", outEvent);
                    } catch (Exception e) {
                        System.err.println("Failed to publish order.cancelled from inventory failure: " + e.getMessage());
                    }
                });
            }
        } catch (Exception ex) {
            System.err.println("Failed to handle inventory reservation failed message: " + ex.getMessage());
        }
    }
}
