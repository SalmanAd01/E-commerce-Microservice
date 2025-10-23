package com.salman_ecommerce.category_template_service.service.impl;

import java.util.ArrayList;
import java.util.List;
import java.util.stream.Collectors;

import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.salman_ecommerce.category_template_service.dto.template.CreateTemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.TemplateDto;
import com.salman_ecommerce.category_template_service.dto.template.UpdateTemplateDto;
import com.salman_ecommerce.category_template_service.entity.Attribute;
import com.salman_ecommerce.category_template_service.entity.Template;
import com.salman_ecommerce.category_template_service.exception.BadRequestException;
import com.salman_ecommerce.category_template_service.exception.ResourceNotFoundException;
import com.salman_ecommerce.category_template_service.mapper.TemplateMapper;
import com.salman_ecommerce.category_template_service.repository.AttributeRepository;
import com.salman_ecommerce.category_template_service.repository.CategoryRepository;
import com.salman_ecommerce.category_template_service.repository.DepartmentRepository;
import com.salman_ecommerce.category_template_service.repository.TemplateRepository;
import com.salman_ecommerce.category_template_service.service.TemplateService;

@Service
public class TemplateServiceImpl implements TemplateService {

    private final TemplateRepository templateRepository;
    private final DepartmentRepository departmentRepository;
    private final CategoryRepository categoryRepository;
    private final AttributeRepository attributeRepository;

    public TemplateServiceImpl(TemplateRepository templateRepository,
                               DepartmentRepository departmentRepository,
                               CategoryRepository categoryRepository,
                               AttributeRepository attributeRepository) {
        this.templateRepository = templateRepository;
        this.departmentRepository = departmentRepository;
        this.categoryRepository = categoryRepository;
        this.attributeRepository = attributeRepository;
    }

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
    public List<TemplateDto> getTemplates(Long departmentId, Long categoryId) {
        List<Template> templates =
                departmentId != null && categoryId != null ? templateRepository.findByDepartmentIdAndCategoryId(departmentId, categoryId) :
                departmentId != null ? templateRepository.findByDepartmentId(departmentId) :
                categoryId != null ? templateRepository.findByCategoryId(categoryId) :
                templateRepository.findAll();

        if (templates.isEmpty()) {
            throw new ResourceNotFoundException("No templates found for the given filters.");
        }

        return templates.stream()
                .map(TemplateMapper::toDto)
                .collect(Collectors.toList());
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