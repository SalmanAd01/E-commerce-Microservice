package com.salman_ecommerce.category_template_service.controllers.v1;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PatchMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.PutMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import com.salman_ecommerce.category_template_service.dto.Department.CreateDepartmentDto;
import com.salman_ecommerce.category_template_service.dto.Department.DepartmentDto;
import com.salman_ecommerce.category_template_service.dto.Department.UpdateDepartmentDto;
import com.salman_ecommerce.category_template_service.services.DepartmentService;



@RestController
@RequestMapping("/api/v1/departments")
public class DepartmentController {
    
    @Autowired
    private DepartmentService departmentService;

    @PostMapping("/")
    public ResponseEntity<DepartmentDto> create(@RequestBody CreateDepartmentDto dto) {
        return new ResponseEntity<>(departmentService.createDepartment(dto), HttpStatus.CREATED);
    }

    @PutMapping("/{id}")
    public ResponseEntity<DepartmentDto> update(@PathVariable Long id, @RequestBody UpdateDepartmentDto dto) {
        return ResponseEntity.ok(departmentService.updateDepartment(id, dto));
    }

    @PatchMapping("/{id}")
    public ResponseEntity<DepartmentDto> patch(@PathVariable Long id, @RequestBody UpdateDepartmentDto dto) {
        return ResponseEntity.ok(departmentService.patchDepartment(id, dto));
    }

    @DeleteMapping("/{id}")
    public ResponseEntity<Void> delete(@PathVariable Long id) {
        departmentService.deleteDepartment(id);
        return ResponseEntity.noContent().build();
    }

    @GetMapping("/{id}")
    public ResponseEntity<DepartmentDto> get(@PathVariable Long id) {
        return ResponseEntity.ok(departmentService.getDepartment(id));
    }

    @GetMapping("/")
    public ResponseEntity<List<DepartmentDto>> getAll() {
        return ResponseEntity.ok(departmentService.getAllDepartments());
    }
    
}
