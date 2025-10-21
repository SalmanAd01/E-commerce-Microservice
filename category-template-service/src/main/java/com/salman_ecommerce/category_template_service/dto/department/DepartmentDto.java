package com.salman_ecommerce.category_template_service.dto.department;

import java.time.LocalDateTime;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class DepartmentDto {
	private Long id;
	private String name;
	private String description;
	private LocalDateTime createdAt;
}

// ...existing code from DepartmentDto.java with updated package...