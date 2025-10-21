package com.salman_ecommerce.category_template_service.controller;

import java.util.List;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.salman_ecommerce.category_template_service.dto.department.CreateDepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.DepartmentDto;
import com.salman_ecommerce.category_template_service.dto.department.UpdateDepartmentDto;
import com.salman_ecommerce.category_template_service.service.DepartmentService;

import io.swagger.v3.oas.annotations.Operation;

@RestController
@RequestMapping("/api/v1/departments")
public class DepartmentController {
    
	private final DepartmentService departmentService;

	public DepartmentController(DepartmentService departmentService) {
		this.departmentService = departmentService;
	}

	@PostMapping("/")
	@Operation(summary = "Create a new department")
	public ResponseEntity<DepartmentDto> create(@RequestBody CreateDepartmentDto dto) {
		return new ResponseEntity<>(departmentService.createDepartment(dto), HttpStatus.CREATED);
	}

	@PutMapping("/{id}")
	@Operation(summary = "Update a department")
	public ResponseEntity<DepartmentDto> update(@PathVariable Long id, @RequestBody UpdateDepartmentDto dto) {
		return ResponseEntity.ok(departmentService.updateDepartment(id, dto));
	}

	@DeleteMapping("/{id}")
	@Operation(summary = "Delete a department")
	public ResponseEntity<Void> delete(@PathVariable Long id) {
		departmentService.deleteDepartment(id);
		return ResponseEntity.noContent().build();
	}

	@GetMapping("/{id}")
	@Operation(summary = "Get department by id")
	public ResponseEntity<DepartmentDto> get(@PathVariable Long id) {
		return ResponseEntity.ok(departmentService.getDepartment(id));
	}

	@GetMapping("/")
	@Operation(summary = "Get all departments")
	public ResponseEntity<List<DepartmentDto>> getAll() {
		return ResponseEntity.ok(departmentService.getAllDepartments());
	}
}