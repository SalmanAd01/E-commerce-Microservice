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

import com.salman_ecommerce.category_template_service.dto.category.CategoryDto;
import com.salman_ecommerce.category_template_service.dto.category.CreateCategoryDto;
import com.salman_ecommerce.category_template_service.dto.category.UpdateCategoryDto;
import com.salman_ecommerce.category_template_service.service.CategoryService;

import io.swagger.v3.oas.annotations.Operation;

@RestController
@RequestMapping("/api/v1/categories")
public class CategoryController {

	private final CategoryService categoryService;

	public CategoryController(CategoryService categoryService) {
		this.categoryService = categoryService;
	}
    
	@GetMapping("/")
	@Operation(summary = "Get all categories")
	public ResponseEntity<List<CategoryDto>> getCategories() {
		List<CategoryDto> categories = categoryService.getCategories();
		return ResponseEntity.ok(categories);
	}
    
	@PostMapping("/")
	@Operation(summary = "Create a new category")
	public ResponseEntity<CategoryDto> createCategory(@RequestBody CreateCategoryDto categoryDto) {
		CategoryDto createdCategory = categoryService.createCategory(categoryDto);
		return ResponseEntity.ok(createdCategory);
	}

	@DeleteMapping("/{id}")
	@Operation(summary = "Delete a category by id")
	public ResponseEntity<Void> deleteCategory(@PathVariable Long id) {
		categoryService.deleteCategory(id);
		return ResponseEntity.noContent().build();
	}

	@GetMapping("/{id}")
	public ResponseEntity<CategoryDto> getCategoryById(@PathVariable Long id) {
		CategoryDto category = categoryService.getCategoryById(id);
		return ResponseEntity.ok(category);
	}
    
	@PutMapping("/{id}")
	public ResponseEntity<CategoryDto> updateCategory(@PathVariable Long id, @RequestBody UpdateCategoryDto updateCategoryDto) {
		CategoryDto updatedCategory = categoryService.updateCategory(id, updateCategoryDto);
		return ResponseEntity.ok(updatedCategory);
	}
}