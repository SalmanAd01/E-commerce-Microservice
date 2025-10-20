package com.salman_ecommerce.category_template_service.services.Impl;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.salman_ecommerce.category_template_service.dto.Template.CreateTemplateDto;
import com.salman_ecommerce.category_template_service.dto.Template.TemplateDto;
import com.salman_ecommerce.category_template_service.dto.Template.UpdateTemplateDto;
import com.salman_ecommerce.category_template_service.entities.Attribute;
import com.salman_ecommerce.category_template_service.entities.Template;
import com.salman_ecommerce.category_template_service.exceptions.BadRequestException;
import com.salman_ecommerce.category_template_service.exceptions.ResourceNotFoundException;
import com.salman_ecommerce.category_template_service.mapper.TemplateMapper;
import com.salman_ecommerce.category_template_service.repositories.AttributeRepository;
import com.salman_ecommerce.category_template_service.repositories.CategoryRepository;
import com.salman_ecommerce.category_template_service.repositories.DepartmentRepository;
import com.salman_ecommerce.category_template_service.repositories.TemplateRepository;
import com.salman_ecommerce.category_template_service.services.TemplateService;

@Service
public class TemplateServiceImpl implements TemplateService {

    @Autowired
    private TemplateRepository templateRepository;

    @Autowired
    private DepartmentRepository departmentRepository;

    @Autowired
    private CategoryRepository categoryRepository;

    @Autowired
    private AttributeRepository attributeRepository;

    @Override
    @Transactional
    public TemplateDto createTemplate(CreateTemplateDto createTemplateDto) {
        if (createTemplateDto.getDepartmentId() == null) {
            throw new BadRequestException("departmentId is required");
        }
        if (createTemplateDto.getCategoryId() == null) {
            throw new BadRequestException("categoryId is required");
        }

        var dept = departmentRepository.findById(createTemplateDto.getDepartmentId())
                .orElseThrow(() -> new ResourceNotFoundException("Department with id '" + createTemplateDto.getDepartmentId() + "' not found."));

        var cat = categoryRepository.findByIdAndLevel(createTemplateDto.getCategoryId(), 3)
                .orElseThrow(() -> new ResourceNotFoundException("Category with id '" + createTemplateDto.getCategoryId() + "' not found for level 3."));

        List<Attribute> attributes = new ArrayList<>();
        if (createTemplateDto.getAttributeIds() != null && !createTemplateDto.getAttributeIds().isEmpty()) {
            attributes = createTemplateDto.getAttributeIds().stream()
                    .map(id -> attributeRepository.findById(id)
                            .orElseThrow(() -> new ResourceNotFoundException("Attribute with id '" + id + "' not found.")))
                    .collect(Collectors.toList());
        }

        Template t = TemplateMapper.toEntity(createTemplateDto, attributes);
        t.setDepartment(dept);
        t.setCategory(cat);

        Template saved = templateRepository.save(t);
        return TemplateMapper.toDto(saved);
    }

    @Override
    @Transactional(readOnly = true)
    public List<TemplateDto> getTemplates() {
        return templateRepository.findAll().stream().map(TemplateMapper::toDto).collect(Collectors.toList());
    }

    @Override
    @Transactional
    public void deleteTemplate(Long id) {
        if (!templateRepository.existsById(id)) {
            throw new ResourceNotFoundException("Template not found with id " + id);
        }
        templateRepository.deleteById(id);
    }

    @Override
    @Transactional(readOnly = true)
    public TemplateDto getTemplateById(Long id) {
        Template t = templateRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Template not found with id " + id));
        return TemplateMapper.toDto(t);
    }

    @Override
    @Transactional
    public TemplateDto updateTemplate(Long id, UpdateTemplateDto updateTemplateDto) {
        Template existing = templateRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Template not found with id " + id));

        existing.setName(updateTemplateDto.getName());
        existing.setDescription(updateTemplateDto.getDescription());

        if (updateTemplateDto.getAttributeIds() != null) {
            List<Attribute> attributes = updateTemplateDto.getAttributeIds().stream()
                    .map(aid -> attributeRepository.findById(aid)
                            .orElseThrow(() -> new ResourceNotFoundException("Attribute with id '" + aid + "' not found.")))
                    .collect(Collectors.toList());
            existing.setAttributes(attributes);
        }

        Template saved = templateRepository.save(existing);
        return TemplateMapper.toDto(saved);
    }
}
