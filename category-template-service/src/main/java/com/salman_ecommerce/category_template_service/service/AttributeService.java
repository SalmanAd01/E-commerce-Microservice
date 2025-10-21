package com.salman_ecommerce.category_template_service.service;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.attribute.AttributeDto;
import com.salman_ecommerce.category_template_service.dto.attribute.CreateAttributeDto;
import com.salman_ecommerce.category_template_service.dto.attribute.UpdateAttributeDto;

public interface AttributeService {
    AttributeDto createAttribute(CreateAttributeDto createAttributeDto);
    List<AttributeDto> getAttributes();
    void deleteAttribute(Long id);
    AttributeDto getAttributeById(Long id);
    AttributeDto updateAttribute(Long id, UpdateAttributeDto updateAttributeDto);
}
