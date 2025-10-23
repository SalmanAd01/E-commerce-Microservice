package com.salman_ecommerce.category_template_service.dto.template;

import java.time.LocalDateTime;
import java.util.List;

import com.salman_ecommerce.category_template_service.entity.Attribute.DataType;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class TemplateDto {
	private Long id;
	private String name;
	private String description;
	private TemplateDepartmentDto department;
	private TemplateCategoryDto category;
	private List<TemplateAttributeDto> attributes;
    private LocalDateTime createdAt;

	@Data
	@NoArgsConstructor
	@AllArgsConstructor
	public static class TemplateAttributeDto {
		private Long id;
		private String name;
		private DataType dataType;
	}

	@Data
	@NoArgsConstructor
	@AllArgsConstructor
	public static class TemplateDepartmentDto {
		private Long id;
		private String name;
	}

	@Data
	@NoArgsConstructor
	@AllArgsConstructor
	public static class TemplateCategoryDto {
		private Long id;
		private String name;
	}

}