package com.example.ecommerce_microservice.order_service.mapper;

import com.example.ecommerce_microservice.order_service.dto.OrderItemDto;
import com.example.ecommerce_microservice.order_service.entity.OrderItem;

public class OrderItemMapper {

    public static OrderItemDto toDto(OrderItem item) {
        if (item == null) return null;
        return OrderItemDto.builder()
                .id(item.getId())
                .productSku(item.getProductSku())
                .storeId(item.getStoreId())
                .quantity(item.getQuantity())
                .price(item.getPrice())
                .totalPrice(item.getTotalPrice())
                .createdAt(item.getCreatedAt())
                .updatedAt(item.getUpdatedAt())
                .build();
    }

    public static OrderItem toEntity(OrderItemDto dto) {
        if (dto == null) return null;
        OrderItem item = new OrderItem();
        item.setId(dto.getId());
        item.setProductSku(dto.getProductSku());
        item.setStoreId(dto.getStoreId());
        item.setQuantity(dto.getQuantity());
        item.setPrice(dto.getPrice());
        item.setTotalPrice(dto.getTotalPrice());
        item.setCreatedAt(dto.getCreatedAt());
        item.setUpdatedAt(dto.getUpdatedAt());
        return item;
    }

}