package com.example.ecommerce_microservice.order_service.dto;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class CreateOrderItemDto {
    private String productSku;
    private Long storeId;
    private Integer quantity;
}
