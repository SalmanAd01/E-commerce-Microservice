package com.example.ecommerce_microservice.order_service.mapper;

import java.util.List;
import java.util.stream.Collectors;

import com.example.ecommerce_microservice.order_service.dto.OrderDto;
import com.example.ecommerce_microservice.order_service.entity.Order;

public class OrderMapper {

    public static OrderDto toDto(Order order) {
        if (order == null) return null;
        List<com.example.ecommerce_microservice.order_service.entity.OrderItem> items = order.getOrderItems();
        return OrderDto.builder()
                .id(order.getId())
                .customerId(order.getCustomerId())
                .totalAmount(order.getTotalAmount())
                .status(order.getStatus())
                .createdAt(order.getCreatedAt())
                .updatedAt(order.getUpdatedAt())
                .orderItems(items == null ? List.of()
                        : items.stream().map(OrderItemMapper::toDto).collect(Collectors.toList()))
                .build();
    }

    public static Order toEntity(OrderDto dto) {
        if (dto == null) return null;
        Order order = new Order();
        order.setId(dto.getId());
        order.setCustomerId(dto.getCustomerId());
        order.setTotalAmount(dto.getTotalAmount());
        order.setStatus(dto.getStatus());
        order.setCreatedAt(dto.getCreatedAt());
        order.setUpdatedAt(dto.getUpdatedAt());
        if (dto.getOrderItems() != null) {
            order.setOrderItems(dto.getOrderItems().stream().map(OrderItemMapper::toEntity).collect(Collectors.toList()));
        }
        return order;
    }

}
