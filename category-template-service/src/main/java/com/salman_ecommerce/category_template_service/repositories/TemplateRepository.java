package com.salman_ecommerce.category_template_service.repositories;

import org.springframework.data.jpa.repository.JpaRepository;

import com.salman_ecommerce.category_template_service.entities.Template;

public interface TemplateRepository extends JpaRepository<Template, Long> {
    boolean existsByCategory_Id(Long categoryId);
}
