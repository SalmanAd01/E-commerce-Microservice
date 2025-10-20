package com.salman_ecommerce.category_template_service.services.Impl;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.salman_ecommerce.category_template_service.dto.Category.CategoryDto;
import com.salman_ecommerce.category_template_service.dto.Category.CreateCategoryDto;
import com.salman_ecommerce.category_template_service.entities.Category;
import com.salman_ecommerce.category_template_service.exceptions.BadRequestException;
import com.salman_ecommerce.category_template_service.exceptions.ResourceNotFoundException;
import com.salman_ecommerce.category_template_service.mapper.CategoryMapper;
import com.salman_ecommerce.category_template_service.repositories.CategoryRepository;
import com.salman_ecommerce.category_template_service.services.CategoryService;
    
@Service
public class CategoryServiceImpl implements CategoryService {

    @Autowired
    private CategoryRepository categoryRepository;
    
    @Autowired
    private com.salman_ecommerce.category_template_service.repositories.DepartmentRepository departmentRepository;
    
    @Autowired
    private com.salman_ecommerce.category_template_service.repositories.TemplateRepository templateRepository;

    @Override
    @Transactional
    public CategoryDto createCategory(CreateCategoryDto categoryDto) {
        if(categoryRepository.existsByName(categoryDto.getName())) {
            throw new BadRequestException("Category with name '" + categoryDto.getName() + "' already exists.");
        }
        Category category = CategoryMapper.toEntity(categoryDto);

        // Resolve and set department
        if (categoryDto.getDepartmentId() != null) {
            var dept = departmentRepository.findById(categoryDto.getDepartmentId())
                    .orElseThrow(() -> new com.salman_ecommerce.category_template_service.exceptions.ResourceNotFoundException(
                            "Department with id '" + categoryDto.getDepartmentId() + "' not found."));
            category.setDepartment(dept);
        }
        // If level is 3, l1 and l2 must be provided
        if (Integer.valueOf(3).equals(categoryDto.getLevel())) {
            if (categoryDto.getL1CategoryId() == null || categoryDto.getL2CategoryId() == null) {
                throw new BadRequestException("Level 3 categories must include both l1CategoryId and l2CategoryId.");
            }
        }

        // If level is 1 or 2, l1 and l2 must NOT be provided
        if (categoryDto.getLevel() != null && (
                Integer.valueOf(1).equals(categoryDto.getLevel()) || Integer.valueOf(2).equals(categoryDto.getLevel()))) {
            if (categoryDto.getL1CategoryId() != null || categoryDto.getL2CategoryId() != null) {
                throw new BadRequestException("Level 1 and 2 categories must not include l1CategoryId or l2CategoryId.");
            }
        }

        // Resolve and set l1 category
        if (categoryDto.getL1CategoryId() != null) {
            var l1 = categoryRepository.findById(categoryDto.getL1CategoryId())
                    .orElseThrow(() -> new com.salman_ecommerce.category_template_service.exceptions.ResourceNotFoundException(
                            "L1 Category with id '" + categoryDto.getL1CategoryId() + "' not found."));
            category.setL1Category(l1);
        }

        // Resolve and set l2 category
        if (categoryDto.getL2CategoryId() != null) {
            var l2 = categoryRepository.findById(categoryDto.getL2CategoryId())
                    .orElseThrow(() -> new com.salman_ecommerce.category_template_service.exceptions.ResourceNotFoundException(
                            "L2 Category with id '" + categoryDto.getL2CategoryId() + "' not found."));
            category.setL2Category(l2);
        }

        // If department is set on new category, ensure l1 and l2 (if present) belong to same department
        if (category.getDepartment() != null) {
            Long deptId = category.getDepartment().getId();
            if (category.getL1Category() != null && category.getL1Category().getDepartment() != null
                    && !deptId.equals(category.getL1Category().getDepartment().getId())) {
                throw new BadRequestException("L1 category must belong to the same department as the new category.");
            }
            if (category.getL2Category() != null && category.getL2Category().getDepartment() != null
                    && !deptId.equals(category.getL2Category().getDepartment().getId())) {
                throw new BadRequestException("L2 category must belong to the same department as the new category.");
            }
        }

        Category savedCategory = categoryRepository.save(category);
        return CategoryMapper.toDto(savedCategory);
    }

    @Override
    @Transactional
    public List<CategoryDto> getCategories() {
        List<Category> categories = categoryRepository.findAll();
        return categories.stream()
                .map(CategoryMapper::toDto)
                .toList();
    }

    @Override
    @Transactional
    public void deleteCategory(Long id) {
        if (!categoryRepository.existsById(id)) {
            throw new ResourceNotFoundException("Category not found with id " + id);
        }
        // Prevent deleting if any templates reference this category
        if (templateRepository.existsByCategory_Id(id)) {
            throw new BadRequestException("Cannot delete category because one or more templates reference it.");
        }

        // Prevent deleting if other categories reference this as l1 or l2 parent
        if (categoryRepository.existsByL1Category_Id(id) || categoryRepository.existsByL2Category_Id(id)) {
            throw new BadRequestException("Cannot delete category because other categories reference it as a parent.");
        }

        categoryRepository.deleteById(id);
    }
    
}
