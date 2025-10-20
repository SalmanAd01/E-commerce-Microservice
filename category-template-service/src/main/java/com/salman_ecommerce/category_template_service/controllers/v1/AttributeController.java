package com.salman_ecommerce.category_template_service.controllers.v1;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.salman_ecommerce.category_template_service.dto.Attribute.AttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.CreateAttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.UpdateAttributeDto;
import com.salman_ecommerce.category_template_service.services.AttributeService;

import io.swagger.v3.oas.annotations.Operation;

@RestController
@RequestMapping("/api/v1/attributes")
public class AttributeController {

    @Autowired
    private AttributeService attributeService;

    @GetMapping("/")
    @Operation(summary = "Get all attributes")
    public ResponseEntity<List<AttributeDto>> getAttributes() {
        return ResponseEntity.ok(attributeService.getAttributes());
    }

    @PostMapping("/")
    @Operation(summary = "Create a new attribute")
    public ResponseEntity<AttributeDto> createAttribute(@RequestBody CreateAttributeDto dto) {
        return ResponseEntity.ok(attributeService.createAttribute(dto));
    }

    @DeleteMapping("/{id}")
    @Operation(summary = "Delete an attribute by id")
    public ResponseEntity<Void> deleteAttribute(@PathVariable Long id) {
        attributeService.deleteAttribute(id);
        return ResponseEntity.noContent().build();
    }

    @GetMapping("/{id}")
    public ResponseEntity<AttributeDto> getAttributeById(@PathVariable Long id) {
        return ResponseEntity.ok(attributeService.getAttributeById(id));
    }

    @PutMapping("/{id}")
    public ResponseEntity<AttributeDto> updateAttribute(@PathVariable Long id, @RequestBody UpdateAttributeDto dto) {
        return ResponseEntity.ok(attributeService.updateAttribute(id, dto));
    }
}
