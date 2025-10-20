package com.salman_ecommerce.category_template_service.services.Impl;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.salman_ecommerce.category_template_service.dto.DepartmentDto;
import com.salman_ecommerce.category_template_service.entities.Department;
import com.salman_ecommerce.category_template_service.mapper.DepartmentMapper;
import com.salman_ecommerce.category_template_service.repositories.DepartmentRepository;
import com.salman_ecommerce.category_template_service.services.DepartmentService;

@Service
public class DepartmentServiceImpl implements DepartmentService {

    @Autowired
    private DepartmentRepository departmentRepository;

    @Override
    public DepartmentDto createDepartment(DepartmentDto departmentDto) {
        Department department = DepartmentMapper.toEntity(departmentDto);
        Department savedDepartment = departmentRepository.save(department);
        departmentDto = DepartmentMapper.toDto(savedDepartment);
        return departmentDto;
    }
}
