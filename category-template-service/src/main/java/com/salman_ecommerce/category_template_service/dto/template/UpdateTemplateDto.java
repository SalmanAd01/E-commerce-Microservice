package com.salman_ecommerce.category_template_service.dto.template;

import java.util.List;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class UpdateTemplateDto {
	private String name;
	private String description;
	private List<Long> attributeIds;
}

// ...existing code from UpdateTemplateDto.java with updated package...