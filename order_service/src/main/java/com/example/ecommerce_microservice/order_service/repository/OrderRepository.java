package com.example.ecommerce_microservice.order_service.repository;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import com.example.ecommerce_microservice.order_service.entity.Order;

@Repository
public interface OrderRepository extends JpaRepository<Order, Long> {
}
