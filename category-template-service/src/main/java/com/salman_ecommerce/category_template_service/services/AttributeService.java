package com.salman_ecommerce.category_template_service.services;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.Attribute.AttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.CreateAttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.UpdateAttributeDto;

public interface AttributeService {
    AttributeDto createAttribute(CreateAttributeDto createAttributeDto);
    List<AttributeDto> getAttributes();
    void deleteAttribute(Long id);
    AttributeDto getAttributeById(Long id);
    AttributeDto updateAttribute(Long id, UpdateAttributeDto updateAttributeDto);
}
