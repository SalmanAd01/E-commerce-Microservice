package com.salman_ecommerce.category_template_service.services.Impl;

import java.util.List;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import com.salman_ecommerce.category_template_service.dto.Department.CreateDepartmentDto;
import com.salman_ecommerce.category_template_service.dto.Department.DepartmentDto;
import com.salman_ecommerce.category_template_service.dto.Department.UpdateDepartmentDto;
import com.salman_ecommerce.category_template_service.entities.Department;
import com.salman_ecommerce.category_template_service.exceptions.BadRequestException;
import com.salman_ecommerce.category_template_service.exceptions.ResourceNotFoundException;
import com.salman_ecommerce.category_template_service.mapper.DepartmentMapper;
import com.salman_ecommerce.category_template_service.repositories.DepartmentRepository;
import com.salman_ecommerce.category_template_service.services.DepartmentService;

import jakarta.transaction.Transactional;

@Service
public class DepartmentServiceImpl implements DepartmentService {

    @Autowired
    private DepartmentRepository departmentRepository;

    @Override
    public DepartmentDto createDepartment(CreateDepartmentDto dto) {
        if (departmentRepository.existsByName(dto.getName())) {
            throw new BadRequestException("Department with name '" + dto.getName() + "' already exists.");
        }
        Department department = DepartmentMapper.toEntity(dto);
        Department saved = departmentRepository.save(department);
        return DepartmentMapper.toDto(saved);
    }

    @Override
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
    public DepartmentDto patchDepartment(Long id, UpdateDepartmentDto dto) {
        Department department = departmentRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Department not found with id " + id));
        DepartmentMapper.updateEntity(dto, department); // selectively update non-null fields
        Department updated = departmentRepository.save(department);
        return DepartmentMapper.toDto(updated);
    }

    @Override
    @Transactional
    public void deleteDepartment(Long id) {
        Department department = departmentRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Department not found with id " + id));
        departmentRepository.delete(department);
    }

    @Override
    public DepartmentDto getDepartment(Long id) {
        Department department = departmentRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Department not found with id " + id));
        return DepartmentMapper.toDto(department);
    }

    @Override
    public List<DepartmentDto> getAllDepartments() {
        return departmentRepository.findAll().stream()
                .map(DepartmentMapper::toDto)
                .collect(Collectors.toList());
    }
}
