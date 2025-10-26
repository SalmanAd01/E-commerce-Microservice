package com.example.ecommerce_microservice.order_service.service.impl;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.example.ecommerce_microservice.order_service.client.InventoryClient;
import com.example.ecommerce_microservice.order_service.common.exception.BadRequestException;
import com.example.ecommerce_microservice.order_service.common.exception.ResourceNotFoundException;
import com.example.ecommerce_microservice.order_service.dto.CreateOrderDto;
import com.example.ecommerce_microservice.order_service.dto.CreateOrderItemDto;
import com.example.ecommerce_microservice.order_service.entity.Order;
import com.example.ecommerce_microservice.order_service.entity.OrderItem;
import com.example.ecommerce_microservice.order_service.events.OrderEventPublisher;
import com.example.ecommerce_microservice.order_service.repository.OrderRepository;
import com.example.ecommerce_microservice.order_service.service.OrderService;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Service
@RequiredArgsConstructor
public class OrderServiceImpl implements OrderService {

    private final OrderRepository orderRepository;
    private final InventoryClient inventoryClient;
    private final OrderEventPublisher eventPublisher;

    @Override
    @Transactional
    public Order createOrder(CreateOrderDto createOrderDto) {
        if (createOrderDto == null) {
            throw new BadRequestException("Request body is required");
        }

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
                double priceToUse = inventoryClient.getSellingPrice(it.getStoreId(), it.getProductSku());

                OrderItem oi = OrderItem.builder()
                        .order(order)
                        .productSku(it.getProductSku())
                        .storeId(it.getStoreId())
                        .quantity(it.getQuantity())
                        .price(priceToUse)
                        .createdAt(LocalDateTime.now())
                        .updatedAt(LocalDateTime.now())
                        .build();
                orderItems.add(oi);
            }
        }

        if (!orderItems.isEmpty()) {
            order.setOrderItems(orderItems);
        }

        Order saved = orderRepository.save(order);
        log.info("Order created with id={} and items={}", saved.getId(),
                saved.getOrderItems() == null ? 0 : saved.getOrderItems().size());
        eventPublisher.publishOrderCreated(saved);
        return saved;
    }

    @Override
    @Transactional
    public Order completeOrder(Long orderId) {
        var order = orderRepository.findById(orderId)
                .orElseThrow(() -> new ResourceNotFoundException("Order not found"));

        if (order.getStatus() != Order.OrderStatus.PAYMENT_SUCCESSFUL) {
            throw new BadRequestException("Order status must be PAYMENT_SUCCESSFUL to complete");
        }

        order.setStatus(Order.OrderStatus.COMPLETED);
        order.setUpdatedAt(LocalDateTime.now());
        Order saved = orderRepository.save(order);
        eventPublisher.publishOrderCompleted(saved);
        return saved;
    }

    @Override
    @Transactional(readOnly = true)
    public Order getOrderById(Long orderId) {
        return orderRepository.findById(orderId)
                .orElseThrow(() -> new ResourceNotFoundException("Order not found"));
    }

    @Override
    @Transactional(readOnly = true)
    public List<Order> getAllOrders() {
        return orderRepository.findAll();
    }

    @Override
    @Transactional
    public void cancelOrder(Long orderId) {
        var order = orderRepository.findById(orderId)
                .orElseThrow(() -> new ResourceNotFoundException("Order not found"));

        if (order.getStatus() == Order.OrderStatus.COMPLETED) {
            throw new BadRequestException("Completed orders cannot be cancelled");
        }
        if (order.getStatus() == Order.OrderStatus.CANCELLED) {
            throw new BadRequestException("Order is already cancelled");
        }

        order.setStatus(Order.OrderStatus.CANCELLED);
        order.setUpdatedAt(LocalDateTime.now());
        Order saved = orderRepository.save(order);
        eventPublisher.publishOrderCancelled(saved, "cancelled_by_user");
    }
}
