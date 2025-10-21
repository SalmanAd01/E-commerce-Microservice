package com.salman_ecommerce.category_template_service.controller;

import java.util.List;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.salman_ecommerce.category_template_service.dto.template.CreateTemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.TemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.UpdateTemplateDto;
import com.salman_ecommerce.category_template_service.service.TemplateService;

import io.swagger.v3.oas.annotations.Operation;

@RestController
@RequestMapping("/api/v1/templates")
public class TemplateController {

	private final TemplateService templateService;

	public TemplateController(TemplateService templateService) {
		this.templateService = templateService;
	}

	@GetMapping("/")
	@Operation(summary = "Get all templates")
	public ResponseEntity<List<TemplateDto>> getTemplates() {
		return ResponseEntity.ok(templateService.getTemplates());
	}

	@PostMapping("/")
	@Operation(summary = "Create a new template")
	public ResponseEntity<TemplateDto> createTemplate(@RequestBody CreateTemplateDto dto) {
		return ResponseEntity.ok(templateService.createTemplate(dto));
	}

	@DeleteMapping("/{id}")
	@Operation(summary = "Delete a template by id")
	public ResponseEntity<Void> deleteTemplate(@PathVariable Long id) {
		templateService.deleteTemplate(id);
		return ResponseEntity.noContent().build();
	}

	@GetMapping("/{id}")
	public ResponseEntity<TemplateDto> getTemplateById(@PathVariable Long id) {
		return ResponseEntity.ok(templateService.getTemplateById(id));
	}

	@PutMapping("/{id}")
	public ResponseEntity<TemplateDto> updateTemplate(@PathVariable Long id, @RequestBody UpdateTemplateDto dto) {
		return ResponseEntity.ok(templateService.updateTemplate(id, dto));
	}
}
// ...existing code from TemplateController.java (with updated package/imports)...