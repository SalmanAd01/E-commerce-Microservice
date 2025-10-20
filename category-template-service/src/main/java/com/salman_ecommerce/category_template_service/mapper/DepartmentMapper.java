package com.salman_ecommerce.category_template_service.mapper;

import com.salman_ecommerce.category_template_service.dto.DepartmentDto;
import com.salman_ecommerce.category_template_service.entities.Department;

public class DepartmentMapper {

    public static DepartmentDto toDto(Department department) {
        DepartmentDto dto = new DepartmentDto();
        dto.setId(department.getDepartmentId());
        dto.setName(department.getName());
        dto.setDescription(department.getDescription());
        dto.setCreatedAt(department.getCreatedAt());
        return dto;
    }   
       

    public static Department toEntity(DepartmentDto dto) {
        Department department = new Department();
        department.setDepartmentId(dto.getId());
        department.setName(dto.getName());
        department.setDescription(dto.getDescription());
        department.setCreatedAt(dto.getCreatedAt());
        return department;
    }
}
