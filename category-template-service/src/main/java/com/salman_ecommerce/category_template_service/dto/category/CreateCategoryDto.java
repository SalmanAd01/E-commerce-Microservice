package com.salman_ecommerce.category_template_service.dto.category;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class CreateCategoryDto {
	private String name;
	private String description;
	private Integer level;
	private Long departmentId;
	private Long l1CategoryId;
	private Long l2CategoryId;
}

// ...existing code from CreateCategoryDto.java with updated package...