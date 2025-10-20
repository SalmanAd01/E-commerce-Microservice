package com.salman_ecommerce.category_template_service.services;

import java.util.List;

import com.salman_ecommerce.category_template_service.dto.Template.CreateTemplateDto;
import com.salman_ecommerce.category_template_service.dto.Template.TemplateDto;
import com.salman_ecommerce.category_template_service.dto.Template.UpdateTemplateDto;

public interface TemplateService {
    TemplateDto createTemplate(CreateTemplateDto createTemplateDto);
    List<TemplateDto> getTemplates();
    void deleteTemplate(Long id);
    TemplateDto getTemplateById(Long id);
    TemplateDto updateTemplate(Long id, UpdateTemplateDto updateTemplateDto);
}
