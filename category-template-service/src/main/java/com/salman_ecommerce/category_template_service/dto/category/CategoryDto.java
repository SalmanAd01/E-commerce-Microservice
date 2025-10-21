package com.salman_ecommerce.category_template_service.dto.category;

import java.time.LocalDateTime;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class CategoryDto {
	private Long id;
	private String name;
	private String description;
	private LocalDateTime createdAt;
	private Integer level;
	private Long departmentId;
	private Long l1CategoryId;
	private Long l2CategoryId;
}
