package com.example.ecommerce_microservice.order_service.client;

import java.util.Map;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestClientException;
import org.springframework.web.client.RestTemplate;

import com.example.ecommerce_microservice.order_service.common.exception.ExternalServiceException;

import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;

@Slf4j
@Component
@RequiredArgsConstructor
public class InventoryClient {

    private final RestTemplate restTemplate;

    @Value("${app.inventory.base-url:http://localhost:5168}")
    private String inventoryBaseUrl;

    /**
     * Fetch the selling price for a given store and product SKU from the inventory service.
     */
    public double getSellingPrice(Long storeId, String productSku) {
        String url = String.format("%s/api/v1/inventories/store/%d/productsku/%s", inventoryBaseUrl, storeId, productSku);
        try {
            ResponseEntity<Map<String, Object>> respEntity = restTemplate.exchange(
                    url,
                    HttpMethod.GET,
                    null,
                    new ParameterizedTypeReference<Map<String, Object>>() {
                    });
            if (respEntity.getStatusCode().is2xxSuccessful() && respEntity.getBody() != null) {
                Object sp = respEntity.getBody().get("sellingPrice");
                if (sp instanceof Number n) {
                    return n.doubleValue();
                }
                if (sp != null) {
                    return Double.parseDouble(sp.toString());
                }
            }
            throw new ExternalServiceException("Inventory service returned unexpected response for SKU " + productSku);
        } catch (RestClientException | ExternalServiceException ex) {
            log.error("InventoryService call failed: {}", ex.getMessage());
            throw new ExternalServiceException("Inventory validation failed: " + ex.getMessage(), ex);
        }
    }
}
