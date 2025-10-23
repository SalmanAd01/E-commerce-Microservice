package com.salman_ecommerce.category_template_service.repository;

import java.util.List;

import org.springframework.data.jpa.repository.JpaRepository;

import com.salman_ecommerce.category_template_service.entity.Template;

public interface TemplateRepository extends JpaRepository<Template, Long> {
    boolean existsByCategory_Id(Long categoryId);
    List<Template> findByDepartmentId(Long departmentId);
    List<Template> findByCategoryId(Long categoryId);
    List<Template> findByDepartmentIdAndCategoryId(Long departmentId, Long categoryId);
}
