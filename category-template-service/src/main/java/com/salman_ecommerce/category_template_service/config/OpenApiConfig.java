package com.salman_ecommerce.category_template_service.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Info;

@Configuration
public class OpenApiConfig {

    @Bean
    public OpenAPI customOpenAPI() {
    return new OpenAPI()
        .openapi("3.0.0")
        .info(new Info().title("Category & Template Service API").version("v1").description("APIs for managing categories and templates"));
    }
}
