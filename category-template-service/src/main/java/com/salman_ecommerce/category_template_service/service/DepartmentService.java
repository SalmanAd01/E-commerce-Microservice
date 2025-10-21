package com.salman_ecommerce.category_template_service.service;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.department.CreateDepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.DepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.UpdateDepartmentDto;

public interface DepartmentService {
    DepartmentDto createDepartment(CreateDepartmentDto dto);
    DepartmentDto updateDepartment(Long id, UpdateDepartmentDto dto);
    void deleteDepartment(Long id);
    DepartmentDto getDepartment(Long id);
    List<DepartmentDto> getAllDepartments();
}
