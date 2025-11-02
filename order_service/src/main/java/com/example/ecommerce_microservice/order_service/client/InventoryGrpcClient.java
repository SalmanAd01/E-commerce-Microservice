package com.example.ecommerce_microservice.order_service.client;

import org.springframework.stereotype.Component;

import com.example.ecommerce_microservice.inventory.pricing.v1.InventoryPricingGrpc;
import com.example.ecommerce_microservice.inventory.pricing.v1.PriceRequest;
import com.example.ecommerce_microservice.inventory.pricing.v1.PriceResponse;
import com.example.ecommerce_microservice.order_service.common.exception.ExternalServiceException;

import io.grpc.ManagedChannel;
import io.grpc.StatusRuntimeException;

@Component
public class InventoryGrpcClient {

    private final InventoryPricingGrpc.InventoryPricingBlockingStub stub;

    public InventoryGrpcClient(ManagedChannel inventoryGrpcChannel) {
        this.stub = InventoryPricingGrpc.newBlockingStub(inventoryGrpcChannel);
    }

    public double getSellingPrice(Long storeId, String productSku) {
        try {
            PriceRequest request = PriceRequest.newBuilder()
                    .setStoreId(storeId)
                    .setProductSku(productSku)
                    .build();
            PriceResponse response = stub.getSellingPrice(request);
            if (!response.getAvailable()) {
                throw new ExternalServiceException("Inventory not available for SKU " + productSku);
            }
            return response.getSellingPrice();
        } catch (StatusRuntimeException e) {
            throw new ExternalServiceException("gRPC call to Inventory service failed: " + e.getStatus(), e);
        }
    }
}
