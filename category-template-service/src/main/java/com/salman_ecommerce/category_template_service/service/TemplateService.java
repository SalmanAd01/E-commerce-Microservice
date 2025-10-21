package com.salman_ecommerce.category_template_service.service;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.template.CreateTemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.TemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.UpdateTemplateDto;

public interface TemplateService {
    TemplateDto createTemplate(CreateTemplateDto createTemplateDto);
    List<TemplateDto> getTemplates();
    void deleteTemplate(Long id);
    TemplateDto getTemplateById(Long id);
    TemplateDto updateTemplate(Long id, UpdateTemplateDto updateTemplateDto);
}
