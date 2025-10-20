package com.salman_ecommerce.category_template_service.controllers.v1;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.salman_ecommerce.category_template_service.entities.Department;
import com.salman_ecommerce.category_template_service.repositories.DepartmentRepository;

import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;

import com.salman_ecommerce.category_template_service.dto.DepartmentDto;
import com.salman_ecommerce.category_template_service.services.DepartmentService;



@RestController
@RequestMapping("/api/v1/departments")
public class DepartmentController {
    
    @Autowired
    private DepartmentRepository departmentRepository;

    @Autowired
    private DepartmentService departmentService;

    @GetMapping("/")
    public List<Department> getDepartments() {
        return departmentRepository.findAll();
    }
    
    @PostMapping("/")
    public ResponseEntity<DepartmentDto> createDepart(@RequestBody DepartmentDto departmentDto) {
        DepartmentDto savedDepartmentDto = departmentService.createDepartment(departmentDto);
        return new ResponseEntity<>(savedDepartmentDto, HttpStatus.CREATED);
    }
    
}
