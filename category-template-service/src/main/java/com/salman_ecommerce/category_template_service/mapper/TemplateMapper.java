package com.salman_ecommerce.category_template_service.mapper;

import java.time.format.DateTimeFormatter;
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
        if (t.getDepartment() != null) {
            dto.setDepartment(new TemplateDto.TemplateDepartmentDto(t.getDepartment().getId(), t.getDepartment().getName()));
        }
        if (t.getCategory() != null) {
            dto.setCategory(new TemplateDto.TemplateCategoryDto(t.getCategory().getId(), t.getCategory().getName()));
        }
        if (t.getAttributes() != null) {
            dto.setAttributes(t.getAttributes().stream()
                    .map(a -> new TemplateDto.TemplateAttributeDto(a.getId(), a.getName(), a.getDataType()))
                    .collect(Collectors.toList()));
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

    /**
     * Map a TemplateDto to gRPC Template message.
     */
    public static com.salman_ecommerce.category_template_service.grpc.Template toProto(TemplateDto dto) {
        var b = com.salman_ecommerce.category_template_service.grpc.Template.newBuilder()
                .setId(dto.getId() == null ? 0L : dto.getId())
                .setName(dto.getName() == null ? "" : dto.getName())
                .setDescription(dto.getDescription() == null ? "" : dto.getDescription());
        if (dto.getDepartment() != null) {
            b.setDepartment(
                    com.salman_ecommerce.category_template_service.grpc.Department.newBuilder()
                            .setId(dto.getDepartment().getId() == null ? 0L : dto.getDepartment().getId())
                            .setName(nullToEmpty(dto.getDepartment().getName()))
                            .build());
        }
        if (dto.getCategory() != null) {
            b.setCategory(
                    com.salman_ecommerce.category_template_service.grpc.Category.newBuilder()
                            .setId(dto.getCategory().getId() == null ? 0L : dto.getCategory().getId())
                            .setName(nullToEmpty(dto.getCategory().getName()))
                            .build());
        }
        if (dto.getAttributes() != null) {
            for (var a : dto.getAttributes()) {
                b.addAttributes(
                        com.salman_ecommerce.category_template_service.grpc.Attribute.newBuilder()
                                .setId(a.getId() == null ? 0L : a.getId())
                                .setName(nullToEmpty(a.getName()))
                                .setDataType(mapType(a.getDataType()))
                                .build());
            }
        }
        if (dto.getCreatedAt() != null) {
            b.setCreatedAt(dto.getCreatedAt().format(DateTimeFormatter.ISO_LOCAL_DATE_TIME));
        }
        return b.build();
    }

    private static com.salman_ecommerce.category_template_service.grpc.DataType mapType(
            com.salman_ecommerce.category_template_service.entity.Attribute.DataType t) {
        if (t == null) return com.salman_ecommerce.category_template_service.grpc.DataType.DATA_TYPE_UNSPECIFIED;
        return switch (t) {
            case TEXT -> com.salman_ecommerce.category_template_service.grpc.DataType.TEXT;
            case NUMBER -> com.salman_ecommerce.category_template_service.grpc.DataType.NUMBER;
            case BOOLEAN -> com.salman_ecommerce.category_template_service.grpc.DataType.BOOLEAN;
        };
    }

    private static String nullToEmpty(String s) {
        return s == null ? "" : s;
    }
}
