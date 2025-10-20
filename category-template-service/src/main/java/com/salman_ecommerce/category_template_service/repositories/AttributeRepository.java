package com.salman_ecommerce.category_template_service.repositories;

import org.springframework.data.jpa.repository.JpaRepository;

import com.salman_ecommerce.category_template_service.entities.Attribute;

public interface AttributeRepository extends JpaRepository<Attribute, Long> {
    boolean existsByNameAndDepartment_Id(String name, Long departmentId);
}
