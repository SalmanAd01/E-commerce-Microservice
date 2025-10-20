package com.salman_ecommerce.category_template_service.repositories;

import org.springframework.data.jpa.repository.JpaRepository;

import com.salman_ecommerce.category_template_service.entities.Category;

public interface CategoryRepository extends JpaRepository<Category, Long> {
    boolean existsByName(String name);
    boolean existsByL1Category_Id(Long l1Id);
    boolean existsByL2Category_Id(Long l2Id);
    java.util.Optional<Category> findByIdAndLevel(Long id, Integer level);
}
