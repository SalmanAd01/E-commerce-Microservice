package com.example.ecommerce_microservice.order_service.service;

import java.util.List;

import com.example.ecommerce_microservice.order_service.dto.CreateOrderDto;
import com.example.ecommerce_microservice.order_service.entity.Order;

public interface OrderService {
    Order createOrder(CreateOrderDto createOrderDto);
    Order completeOrder(Long orderId);
    Order getOrderById(Long orderId);
    List<Order> getAllOrders();
    void cancelOrder(Long orderId);
}