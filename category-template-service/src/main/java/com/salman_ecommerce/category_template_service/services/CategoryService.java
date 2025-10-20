package com.salman_ecommerce.category_template_service.services;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.Category.CategoryDto;
import com.salman_ecommerce.category_template_service.dto.Category.CreateCategoryDto;

public interface CategoryService {
    CategoryDto createCategory(CreateCategoryDto dto);
    List<CategoryDto> getCategories();
    void deleteCategory(Long id);
}
