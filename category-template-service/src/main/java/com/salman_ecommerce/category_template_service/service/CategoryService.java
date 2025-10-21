package com.salman_ecommerce.category_template_service.service;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.category.CategoryDto;
import com.salman_ecommerce.category_template_service.dto.category.CreateCategoryDto;
import com.salman_ecommerce.category_template_service.dto.category.UpdateCategoryDto;

public interface CategoryService {
    CategoryDto createCategory(CreateCategoryDto dto);
    List<CategoryDto> getCategories();
    void deleteCategory(Long id);
    CategoryDto getCategoryById(Long id);
    CategoryDto updateCategory(Long id, UpdateCategoryDto updateCategoryDto);
}
