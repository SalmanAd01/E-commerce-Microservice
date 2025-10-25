package com.example.ecommerce_microservice.order_service.controller.v1;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.example.ecommerce_microservice.order_service.dto.CreateOrderDto;
import com.example.ecommerce_microservice.order_service.dto.OrderDto;
import com.example.ecommerce_microservice.order_service.entity.Order;
import com.example.ecommerce_microservice.order_service.mapper.OrderMapper;
import com.example.ecommerce_microservice.order_service.service.OrderService;

@RestController
@RequestMapping("/api/v1/orders")
public class OrderController {

    @GetMapping("/")
    public String getOrders() {
        return "List of orders";
    }

    private final OrderService orderService;

    public OrderController(OrderService orderService) {
        this.orderService = orderService;
    }

    @PostMapping("/")
    public ResponseEntity<OrderDto> createOrder(@RequestBody CreateOrderDto body) {
        Order saved = orderService.createOrder(body);
        OrderDto dto = OrderMapper.toDto(saved);
        return ResponseEntity.ok(dto);
    }
}
