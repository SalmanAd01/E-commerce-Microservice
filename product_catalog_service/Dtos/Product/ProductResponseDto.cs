using System;
using System.Collections.Generic;

namespace product_catalog_service.Dtos.Product
{
    public class ProductResponseDto
    {
        public string? Id { get; set; }
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryId { get; set; } = default!;
        public string DepartmentId { get; set; } = default!;
        public string TemplateId { get; set; } = default!;
        public string BrandId { get; set; } = default!;
        public List<ProductAttributeResponseDto>? Attributes { get; set; }
        public List<VariantResponseDto> Variants { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ProductAttributeResponseDto
    {
        public int AttributeId { get; set; }
        public object? Value { get; set; }
    }

    public class VariantResponseDto
    {
        public string VariantId { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal ActualPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public string Sku { get; set; } = default!;
    }
}
