package com.salman_ecommerce.category_template_service.services.Impl;

import java.util.List;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import com.salman_ecommerce.category_template_service.dto.Attribute.AttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.CreateAttributeDto;
import com.salman_ecommerce.category_template_service.dto.Attribute.UpdateAttributeDto;
import com.salman_ecommerce.category_template_service.entities.Attribute;
import com.salman_ecommerce.category_template_service.exceptions.BadRequestException;
import com.salman_ecommerce.category_template_service.exceptions.ResourceNotFoundException;
import com.salman_ecommerce.category_template_service.mapper.AttributeMapper;
import com.salman_ecommerce.category_template_service.repositories.AttributeRepository;

@Service
public class AttributeServiceImpl implements com.salman_ecommerce.category_template_service.services.AttributeService {

    @Autowired
    private AttributeRepository attributeRepository;

    @Autowired
    private com.salman_ecommerce.category_template_service.repositories.DepartmentRepository departmentRepository;

    @Autowired
    private com.salman_ecommerce.category_template_service.repositories.TemplateRepository templateRepository;

    @Override
    @Transactional
    public AttributeDto createAttribute(CreateAttributeDto createAttributeDto) {
        if (createAttributeDto.getDepartmentId() == null) {
            throw new BadRequestException("departmentId is required");
        }
        if (attributeRepository.existsByNameAndDepartment_Id(createAttributeDto.getName(), createAttributeDto.getDepartmentId())) {
            throw new BadRequestException("Attribute with name '" + createAttributeDto.getName() + "' already exists in the department.");
        }

        var dept = departmentRepository.findById(createAttributeDto.getDepartmentId())
                .orElseThrow(() -> new ResourceNotFoundException("Department with id '" + createAttributeDto.getDepartmentId() + "' not found."));

        Attribute attribute = AttributeMapper.toEntity(createAttributeDto);
        attribute.setDepartment(dept);

        Attribute saved = attributeRepository.save(attribute);
        return AttributeMapper.toDto(saved);
    }

    @Override
    @Transactional(readOnly = true)
    public List<AttributeDto> getAttributes() {
        return attributeRepository.findAll().stream().map(AttributeMapper::toDto).toList();
    }

    @Override
    @Transactional
    public void deleteAttribute(Long id) {
        if (!attributeRepository.existsById(id)) {
            throw new ResourceNotFoundException("Attribute not found with id " + id);
        }
        if (templateRepository.existsById(id)) {
            throw new BadRequestException("Cannot delete attribute because one or more templates reference it.");
        }

        boolean usedInTemplates = templateRepository.findAll().stream()
                .anyMatch(t -> t.getAttributes() != null && t.getAttributes().stream().anyMatch(a -> a.getId().equals(id)));
        if (usedInTemplates) {
            throw new BadRequestException("Cannot delete attribute because one or more templates reference it.");
        }

        attributeRepository.deleteById(id);
    }

    @Override
    @Transactional(readOnly = true)
    public AttributeDto getAttributeById(Long id) {
        Attribute attribute = attributeRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Attribute not found with id " + id));
        return AttributeMapper.toDto(attribute);
    }

    @Override
    @Transactional
    public AttributeDto updateAttribute(Long id, UpdateAttributeDto updateAttributeDto) {
        Attribute existing = attributeRepository.findById(id)
                .orElseThrow(() -> new ResourceNotFoundException("Attribute not found with id " + id));

    existing.setName(updateAttributeDto.getName());

        Attribute saved = attributeRepository.save(existing);
        return AttributeMapper.toDto(saved);
    }
}
