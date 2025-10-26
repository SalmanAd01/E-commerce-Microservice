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

    @KafkaListener(topics = {"inventory.reservation_failed", "inventory.reserved"}, groupId = "order-service-group")
    public void onInventoryReservationFailed(String message) {
        try {
            @SuppressWarnings("unchecked")
            Map<String, Object> event = objectMapper.readValue(message, Map.class);

            if (event.get("orderId") != null) {
                Long parsedOrderId = Long.valueOf(String.valueOf(event.get("orderId")));
                final Long orderIdFinal = parsedOrderId;
                orderRepository.findById(orderIdFinal).ifPresent(order -> {
                    if ("inventory.reserved".equals(event.get("eventType"))) {
                        order.setStatus(Order.OrderStatus.INVENTORY_RESERVED);
                    } else {
                        order.setStatus(Order.OrderStatus.INVENTORY_RESERVATION_FAILED);
                    }
                    orderRepository.save(order);
                });
            }
        } catch (Exception ex) {
            System.err.println("Failed to handle inventory reservation failed message: " + ex.getMessage());
        }
    }
}
