package com.salman_ecommerce.category_template_service.mapper;

import java.util.List;
import java.util.stream.Collectors;

import com.salman_ecommerce.category_template_service.dto.template.CreateTemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.TemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.UpdateTemplateDto;
import com.salman_ecommerce.category_template_service.entity.Attribute;
import com.salman_ecommerce.category_template_service.entity.Template;

public class TemplateMapper {
    public static TemplateDto toDto(Template t) {
        TemplateDto dto = new TemplateDto();
        dto.setId(t.getId());
        dto.setName(t.getName());
        dto.setDescription(t.getDescription());
        dto.setDepartmentId(t.getDepartment() != null ? t.getDepartment().getId() : null);
        dto.setCategoryId(t.getCategory() != null ? t.getCategory().getId() : null);
        if (t.getAttributes() != null) {
            dto.setAttributeIds(t.getAttributes().stream().map(Attribute::getId).collect(Collectors.toList()));
        }
        dto.setCreatedAt(t.getCreatedAt());
        return dto;
    }

    public static Template toEntity(CreateTemplateDto dto, List<Attribute> attributes) {
        Template t = new Template();
        t.setName(dto.getName());
        t.setDescription(dto.getDescription());
        t.setAttributes(attributes);
        return t;
    }

    public static Template toEntity(UpdateTemplateDto dto, List<Attribute> attributes) {
        Template t = new Template();
        t.setName(dto.getName());
        t.setDescription(dto.getDescription());
        t.setAttributes(attributes);
        return t;
    }
}
