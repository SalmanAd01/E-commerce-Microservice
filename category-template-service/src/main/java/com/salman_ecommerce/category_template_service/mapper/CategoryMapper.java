package com.salman_ecommerce.category_template_service.mapper;

import com.salman_ecommerce.category_template_service.dto.category.CategoryDto;
import com.salman_ecommerce.category_template_service.dto.category.CreateCategoryDto;
import com.salman_ecommerce.category_template_service.dto.category.UpdateCategoryDto;
import com.salman_ecommerce.category_template_service.entity.Category;

public class CategoryMapper {
    public static CategoryDto toDto(Category category) {
        CategoryDto categoryDto = new CategoryDto();
        categoryDto.setId(category.getId());
        categoryDto.setName(category.getName());
        categoryDto.setDescription(category.getDescription());
        categoryDto.setLevel(category.getLevel());
        categoryDto.setCreatedAt(category.getCreatedAt());
        categoryDto.setDepartmentId(category.getDepartment() != null ? category.getDepartment().getId() : null);
        categoryDto.setL1CategoryId(category.getL1Category() != null ? category.getL1Category().getId() : null);
        categoryDto.setL2CategoryId(category.getL2Category() != null ? category.getL2Category().getId() : null);

        return categoryDto;
    }

    public static Category toEntity(CreateCategoryDto createCategoryDto) {
        Category category = new Category();
        category.setName(createCategoryDto.getName());
        category.setDescription(createCategoryDto.getDescription());
        category.setLevel(createCategoryDto.getLevel());
        return category;
    }

    public static Category toEntity(UpdateCategoryDto updateCategoryDto) {
        Category category = new Category();
        category.setName(updateCategoryDto.getName());
        category.setDescription(updateCategoryDto.getDescription());
        return category;
    }
}
