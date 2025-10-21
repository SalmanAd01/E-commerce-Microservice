package com.salman_ecommerce.category_template_service.service.impl;

import java.util.List;
import java.util.stream.Collectors;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.salman_ecommerce.category_template_service.dto.department.CreateDepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.DepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.UpdateDepartmentDto;
import com.salman_ecommerce.category_template_service.entity.Department;
import com.salman_ecommerce.category_template_service.exception.BadRequestException;
import com.salman_ecommerce.category_template_service.exception.ResourceNotFoundException;
import com.salman_ecommerce.category_template_service.mapper.DepartmentMapper;
import com.salman_ecommerce.category_template_service.repository.DepartmentRepository;
import com.salman_ecommerce.category_template_service.service.DepartmentService;

@Service
public class DepartmentServiceImpl implements DepartmentService {

    private final DepartmentRepository departmentRepository;

    public DepartmentServiceImpl(DepartmentRepository departmentRepository) {
        this.departmentRepository = departmentRepository;
    }

    @Override
    @Transactional
    public DepartmentDto createDepartment(CreateDepartmentDto dto) {
        if (departmentRepository.existsByName(dto.getName())) {
            throw new BadRequestException("Department with name '" + dto.getName() + "' already exists.");
        }
        Department department = DepartmentMapper.toEntity(dto);
        Department saved = departmentRepository.save(department);
        return DepartmentMapper.toDto(saved);
    }

    @Override
    @Transactional
    public DepartmentDto updateDepartment(Long id, UpdateDepartmentDto dto) {
        Department department = departmentRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Department not found with id " + id));
        // Overwrite all fields for PUT
        if (dto.getName() != null) department.setName(dto.getName());
        if (dto.getDescription() != null) department.setDescription(dto.getDescription());
        Department updated = departmentRepository.save(department);
        return DepartmentMapper.toDto(updated);
    }

    @Override
    @Transactional
    public void deleteDepartment(Long id) {
        if (!departmentRepository.existsById(id)) {
            throw new ResourceNotFoundException("Department not found with id " + id);
        }
        departmentRepository.deleteById(id);
    }

    @Override   
    @Transactional(readOnly = true)
    public DepartmentDto getDepartment(Long id) {
        Department department = departmentRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Department not found with id " + id));
        return DepartmentMapper.toDto(department);
    }

    @Override
    @Transactional(readOnly = true)
    public List<DepartmentDto> getAllDepartments() {
        return departmentRepository.findAll().stream()
                .map(DepartmentMapper::toDto)
                .collect(Collectors.toList());
    }
}