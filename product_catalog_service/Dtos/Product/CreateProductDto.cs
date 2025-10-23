using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using product_catalog_service.Dtos.Product.ProductAttribute;
using product_catalog_service.Dtos.Product.Variant;
using System.Text.Json.Serialization;
using product_catalog_service.Models;

namespace product_catalog_service.Dtos.Product
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
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ProductUnit? Unit { get; set; }
        public required List<VariantDto> Variants { get; set; }
    }
}