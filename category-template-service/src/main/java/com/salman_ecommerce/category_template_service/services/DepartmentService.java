package com.salman_ecommerce.category_template_service.services;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.Department.CreateDepartmentDto;
import com.salman_ecommerce.category_template_service.dto.Department.DepartmentDto;
import com.salman_ecommerce.category_template_service.dto.Department.UpdateDepartmentDto;

public interface DepartmentService {
    DepartmentDto createDepartment(CreateDepartmentDto dto);
    DepartmentDto updateDepartment(Long id, UpdateDepartmentDto dto);
    DepartmentDto patchDepartment(Long id, UpdateDepartmentDto dto);
    void deleteDepartment(Long id);
    DepartmentDto getDepartment(Long id);
    List<DepartmentDto> getAllDepartments();
}
