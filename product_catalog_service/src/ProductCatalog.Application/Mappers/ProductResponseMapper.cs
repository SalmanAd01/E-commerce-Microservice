using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using ProductCatalog.Application.Dtos.Product;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Mappers
{
    public static class ProductResponseMapper
    {
        public static ProductResponseDto ToResponse(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));

            return new ProductResponseDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId,
                DepartmentId = product.DepartmentId,
                TemplateId = product.TemplateId,
                BrandId = product.BrandId,
                Attributes = product.Attributes?.Select(a => new ProductAttributeResponseDto { AttributeId = a.AttributeId, Value = BsonToClr(a.Value) }).ToList(),
                Variants = product.Variants?.Select(v => new VariantResponseDto { VariantId = v.VariantId, Name = v.Name, Sku = v.Sku }).ToList() ?? new List<VariantResponseDto>(),
                Unit = product.Unit,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        private static object? BsonToClr(BsonValue? val)
        {
            if (val == null || val.IsBsonNull) return null;

            return val.BsonType switch
            {
                BsonType.String => val.AsString,
                BsonType.Boolean => val.AsBoolean,
                BsonType.Int32 => val.AsInt32,
                BsonType.Int64 => val.AsInt64,
                BsonType.Double => val.AsDouble,
                BsonType.Decimal128 => val.ToDecimal(),
                BsonType.DateTime => val.ToUniversalTime(),
                BsonType.Document => val.AsBsonDocument.ToDictionary(),
                BsonType.Array => val.AsBsonArray.Select(BsonToClr).ToList(),
                _ => val.ToString()
            };
        }

        private static Dictionary<string, object?> ToDictionary(this BsonDocument doc)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var elem in doc.Elements)
            {
                dict[elem.Name] = BsonToClr(elem.Value);
            }
            return dict;
        }
    }
}
