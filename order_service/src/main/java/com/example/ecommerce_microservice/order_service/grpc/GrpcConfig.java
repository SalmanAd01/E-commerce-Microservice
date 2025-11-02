package com.example.ecommerce_microservice.order_service.grpc;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import io.grpc.ManagedChannel;
import io.grpc.ManagedChannelBuilder;

@Configuration
public class GrpcConfig {

    @Value("${app.inventory.grpc.host:localhost}")
    private String grpcHost;

    @Value("${app.inventory.grpc.port:5169}")
    private int grpcPort;

    @Bean(destroyMethod = "shutdownNow")
    public ManagedChannel inventoryGrpcChannel() {
        return ManagedChannelBuilder.forAddress(grpcHost, grpcPort)
                .usePlaintext()
                .build();
    }
}
