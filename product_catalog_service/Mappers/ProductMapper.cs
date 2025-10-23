using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using product_catalog_service.Dtos.Product;
using product_catalog_service.Dtos.Product.ProductAttribute;
using product_catalog_service.Dtos.Product.Variant;
using product_catalog_service.Models;

namespace product_catalog_service.Mappers
{
    public static class ProductMapper
    {
        public static Product ToProduct(CreateProductDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var product = new Product
            {
                Name = dto.Name,
                Slug = dto.Slug,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId,
                DepartmentId = dto.DepartmentId,
                TemplateId = dto.TemplateId,
                BrandId = dto.BrandId,
                Attributes = dto.Attributes?.Select(a => new ProductAttribute { AttributeId = a.AttributeId, Value = BsonValueFromObject(a.Value) }).ToList(),
                Variants = dto.Variants.Select(v => new Variant { Name = v.Name, ActualPrice = v.ActualPrice, SellingPrice = v.SellingPrice ?? v.ActualPrice, Sku = v.Sku }).ToList()
            };

            return product;
        }

        public static BsonValue? BsonValueFromObject(object? value)
        {
            if (value == null) return BsonNull.Value;
            if (value is BsonValue bv) return bv;
            if (value is string s) return BsonValue.Create(s);
            if (value is bool b) return BsonValue.Create(b);
            if (value is int i) return BsonValue.Create(i);
            if (value is long l) return BsonValue.Create(l);
            if (value is double d) return BsonValue.Create(d);
            if (value is decimal dec) return BsonValue.Create(Convert.ToDouble(dec));
            if (value is System.Text.Json.JsonElement je)
            {
                return je.ValueKind switch
                {
                    System.Text.Json.JsonValueKind.String => BsonValue.Create(je.GetString()),
                    System.Text.Json.JsonValueKind.Number => BsonValue.Create(je.GetDouble()),
                    System.Text.Json.JsonValueKind.True => BsonValue.Create(true),
                    System.Text.Json.JsonValueKind.False => BsonValue.Create(false),
                    _ => BsonNull.Value
                };
            }

            return BsonValue.Create(value.ToString());
        }
    }
}
