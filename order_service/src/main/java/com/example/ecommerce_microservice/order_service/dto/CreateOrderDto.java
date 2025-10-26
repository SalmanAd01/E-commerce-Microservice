package com.example.ecommerce_microservice.order_service.dto;

import java.util.List;

import jakarta.validation.Valid;
import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class CreateOrderDto {
    @NotNull(message = "customerId is required")
    private Long customerId;

    @NotEmpty(message = "items must not be empty")
    private List<@Valid CreateOrderItemDto> items;
}
