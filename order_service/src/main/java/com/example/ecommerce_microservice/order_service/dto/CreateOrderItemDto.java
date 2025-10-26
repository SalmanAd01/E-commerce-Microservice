package com.example.ecommerce_microservice.order_service.dto;

import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class CreateOrderItemDto {
    @NotBlank(message = "productSku is required")
    private String productSku;

    @NotNull(message = "storeId is required")
    private Long storeId;

    @NotNull
    @Min(value = 1, message = "quantity must be at least 1")
    private Integer quantity;
}
