package com.salman_ecommerce.category_template_service.dto.attribute;

import com.salman_ecommerce.category_template_service.entity.Attribute.DataType;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class CreateAttributeDto {
	private Long departmentId;
	private String name;
	private DataType dataType;
}
