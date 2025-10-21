package com.salman_ecommerce.category_template_service.mapper;

import com.salman_ecommerce.category_template_service.dto.department.CreateDepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.DepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.UpdateDepartmentDto;
import com.salman_ecommerce.category_template_service.entity.Department;

public class DepartmentMapper {

    public static DepartmentDto toDto(Department department) {
        DepartmentDto dto = new DepartmentDto();
        dto.setId(department.getId());
        dto.setName(department.getName());
        dto.setDescription(department.getDescription());
        dto.setCreatedAt(department.getCreatedAt());
        return dto;
    }

    public static Department toEntity(CreateDepartmentDto dto) {
        Department department = new Department();
        department.setName(dto.getName());
        department.setDescription(dto.getDescription());
        return department;
    }

    public static void updateEntity(UpdateDepartmentDto dto, Department entity) {
        if (dto.getName() != null) entity.setName(dto.getName());
        if (dto.getDescription() != null) entity.setDescription(dto.getDescription());
    }
}
