package com.salman_ecommerce.category_template_service.dto.attribute;

import java.time.LocalDateTime;

import com.salman_ecommerce.category_template_service.entity.Attribute.DataType;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class AttributeDto {
	private Long id;
	private Long departmentId;
	private String name;
	private DataType dataType;
	private LocalDateTime createdAt;
}

// ...existing code from AttributeDto.java with updated package...