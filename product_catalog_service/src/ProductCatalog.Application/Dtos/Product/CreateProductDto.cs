using System.Collections.Generic;
using System.Text.Json.Serialization;
using ProductCatalog.Application.Dtos.Product.ProductAttribute;
using ProductCatalog.Application.Dtos.Product.Variant;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Dtos.Product
{
    public class CreateProductDto
    {
        public required string Name { get; set; }
        public required string Slug { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public required string CategoryId { get; set; }
        public required string DepartmentId { get; set; }
        public required string TemplateId { get; set; }
        public required string BrandId { get; set; }
        public List<ProductAttributeDto>? Attributes { get; set; } = new();
    [JsonConverter(typeof(ProductCatalog.Domain.Converters.ProductUnitJsonConverter))]
        public ProductUnit? Unit { get; set; }
        public required List<VariantDto> Variants { get; set; }
    }
}
