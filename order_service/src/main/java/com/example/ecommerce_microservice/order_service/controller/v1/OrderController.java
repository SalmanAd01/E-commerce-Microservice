package com.example.ecommerce_microservice.order_service.controller.v1;

import java.util.List;
import java.util.stream.Collectors;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.example.ecommerce_microservice.order_service.dto.CreateOrderDto;
import com.example.ecommerce_microservice.order_service.dto.OrderDto;
import com.example.ecommerce_microservice.order_service.entity.Order;
import com.example.ecommerce_microservice.order_service.mapper.OrderMapper;
import com.example.ecommerce_microservice.order_service.service.OrderService;

import jakarta.validation.Valid;

@RestController
@RequestMapping("/api/v1/orders")
public class OrderController {

    @GetMapping("")
    public ResponseEntity<List<OrderDto>> getOrders() {
        List<Order> orders = orderService.getAllOrders();
        List<OrderDto> dtos = orders.stream()
                .map(OrderMapper::toDto)
                .collect(Collectors.toList());
        return ResponseEntity.ok(dtos);
    }

    @GetMapping("/{orderId}")
    public ResponseEntity<OrderDto> getOrderById(@PathVariable("orderId") Long orderId) {
        Order order = orderService.getOrderById(orderId);
        OrderDto dto = OrderMapper.toDto(order);
        return ResponseEntity.ok(dto);
    }

    private final OrderService orderService;

    public OrderController(OrderService orderService) {
        this.orderService = orderService;
    }

    @PostMapping("")
    public ResponseEntity<OrderDto> createOrder(@Valid @RequestBody CreateOrderDto body) {
        Order saved = orderService.createOrder(body);
        OrderDto dto = OrderMapper.toDto(saved);
        return ResponseEntity.status(HttpStatus.CREATED).body(dto);
    }

    @PostMapping("/{orderId}/complete")
    public ResponseEntity<OrderDto> completeOrder(@PathVariable("orderId") Long orderId) {
        Order saved = orderService.completeOrder(orderId);
        return ResponseEntity.ok(OrderMapper.toDto(saved));
    }

    @PostMapping("/{orderId}/cancel")
    public ResponseEntity<Void> cancelOrder(@PathVariable("orderId") Long orderId) {
        orderService.cancelOrder(orderId);
        return ResponseEntity.noContent().build();
    }
    
}
