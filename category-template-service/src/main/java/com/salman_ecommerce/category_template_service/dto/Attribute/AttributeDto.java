package com.salman_ecommerce.category_template_service.dto.Attribute;

import java.time.LocalDateTime;

import com.salman_ecommerce.category_template_service.entities.Attribute.DataType;

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
