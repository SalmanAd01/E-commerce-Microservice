package com.salman_ecommerce.category_template_service.repositories;

import org.springframework.data.jpa.repository.JpaRepository;

import com.salman_ecommerce.category_template_service.entities.Department;

public interface DepartmentRepository extends JpaRepository<Department, Long> {
    boolean existsByName(String name);
}
