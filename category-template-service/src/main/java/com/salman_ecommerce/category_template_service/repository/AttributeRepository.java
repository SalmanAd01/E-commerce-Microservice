package com.salman_ecommerce.category_template_service.repository;

import org.springframework.data.jpa.repository.JpaRepository;

import com.salman_ecommerce.category_template_service.entity.Attribute;

public interface AttributeRepository extends JpaRepository<Attribute, Long> {
    boolean existsByNameAndDepartment_Id(String name, Long departmentId);
}
