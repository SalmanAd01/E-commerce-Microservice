using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using ProductCatalog.Domain.Attributes;

namespace ProductCatalog.Domain.Entities
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")] 
        public required string Name { get; set; }

        [MongoIndex(Unique = true)]
        [BsonElement("slug")] 
        public required string Slug { get; set; }

        [BsonElement("description")] 
        public string? Description { get; set; }

        [BsonElement("imageUrl")] 
        public string? ImageUrl { get; set; }

        [BsonElement("category_id")] 
        public required string CategoryId { get; set; }

        [BsonElement("department_id")] 
        public required string DepartmentId { get; set; }

        [BsonElement("template_id")] 
        public required string TemplateId { get; set; }

        [BsonElement("brand_id")] 
        public required string BrandId { get; set; }

        [BsonElement("unit")]
        [BsonRepresentation(BsonType.String)]
    [JsonConverter(typeof(ProductCatalog.Domain.Converters.ProductUnitJsonConverter))]
        public ProductUnit? Unit { get; set; }

        [BsonElement("attributes")] 
        public List<ProductAttribute>? Attributes { get; set; } = new();

        [BsonElement("variants")] 
        public required List<Variant> Variants { get; set; }

        [BsonElement("created_at")] 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")] 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Variant
    {
        public string VariantId { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("name")] 
        public required string Name { get; set; }

        [BsonElement("sku")] 
        public required string Sku { get; set; }
    }

    public class ProductAttribute
    {
        [BsonElement("attribute_id")] 
        public int AttributeId { get; set; }

        [BsonElement("value")] 
        public BsonValue? Value { get; set; }
    }

    public enum ProductUnit
    {
        Item,
        Kg
    }
}
