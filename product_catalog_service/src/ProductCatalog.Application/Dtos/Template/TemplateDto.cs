using System;
using System.Collections.Generic;

namespace ProductCatalog.Application.Dtos.Template
{
    public class TemplateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DepartmentDto? Department { get; set; }
        public CategoryDto? Category { get; set; }
        public List<TemplateAttributeDto>? Attributes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class TemplateAttributeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? DataType { get; set; }
    }
}
