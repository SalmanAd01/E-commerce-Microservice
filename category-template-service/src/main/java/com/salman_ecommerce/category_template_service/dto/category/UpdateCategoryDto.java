package com.salman_ecommerce.category_template_service.dto.category;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class UpdateCategoryDto {
	private String name;
	private String description;
}

// ...existing code from UpdateCategoryDto.java with updated package...