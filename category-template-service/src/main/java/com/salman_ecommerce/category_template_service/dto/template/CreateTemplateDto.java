package com.salman_ecommerce.category_template_service.dto.template;

import java.util.List;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class CreateTemplateDto {
	private String name;
	private String description;
	private Long departmentId;
	private Long categoryId;
	private List<Long> attributeIds;
}
