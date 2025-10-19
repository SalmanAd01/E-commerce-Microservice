package com.salman_ecommerce.category_template_service.controllers;

import org.springframework.boot.web.servlet.error.ErrorController;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

import jakarta.servlet.http.HttpServletRequest;


@RestController
public class CustomErrorController implements ErrorController {
    
    @GetMapping("/error")
    public String handleError(HttpServletRequest request) {
        Integer status = (Integer) request.getAttribute("jakarta.servlet.error.status_code");
        if (status == 404) {
            return "Custom 404: Page not found!";
        }
        return "Error occurred. Status: " + status;
    }
    
}
