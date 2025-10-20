package com.salman_ecommerce.category_template_service.mapper;

import com.salman_ecommerce.category_template_service.dto.Attribute.AttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.CreateAttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.UpdateAttributeDto;
import com.salman_ecommerce.category_template_service.entities.Attribute;

public class AttributeMapper {
    public static AttributeDto toDto(Attribute attribute) {
        AttributeDto dto = new AttributeDto();
        dto.setId(attribute.getId());
        dto.setDepartmentId(attribute.getDepartment() != null ? attribute.getDepartment().getId() : null);
        dto.setName(attribute.getName());
        dto.setDataType(attribute.getDataType());
        dto.setCreatedAt(attribute.getCreatedAt());
        return dto;
    }

    public static Attribute toEntity(CreateAttributeDto dto) {
        Attribute attr = new Attribute();
        attr.setName(dto.getName());
        attr.setDataType(dto.getDataType());
        return attr;
    }

    public static Attribute toEntity(UpdateAttributeDto dto) {
        Attribute attr = new Attribute();
        attr.setName(dto.getName());
        // Do not change dataType on update via UpdateAttributeDto
        return attr;
    }
}
