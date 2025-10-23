using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using product_catalog_service.Dtos.Product;
using product_catalog_service.Dtos.Template;
using product_catalog_service.Mappers;
using product_catalog_service.Models;
using product_catalog_service.Repositories;

namespace product_catalog_service.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandService _brandService;
        private readonly ITemplateService _templateService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, IBrandService brandService, ITemplateService templateService, ILogger<ProductService> logger)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _brandService = brandService ?? throw new ArgumentNullException(nameof(brandService));
            _templateService = templateService ?? throw new ArgumentNullException(nameof(templateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Product> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            // validate brand exists
            var brand = await _brandService.GetBrandByIdAsync(dto.BrandId).ConfigureAwait(false);
            if (brand == null) throw new ArgumentException($"Brand with id '{dto.BrandId}' not found");

            // get template
            var template = await _templateService.GetTemplateByIdAsync(dto.TemplateId, cancellationToken).ConfigureAwait(false);
            if (template == null) throw new ArgumentException($"Template with id '{dto.TemplateId}' not found or template service unavailable");

            // validate department and category
            if (template.Department == null || template.Department.Id.ToString() != dto.DepartmentId)
            {
                throw new ArgumentException("Department id does not match template");
            }

            if (template.Category == null || template.Category.Id.ToString() != dto.CategoryId)
            {
                throw new ArgumentException("Category id does not match template");
            }

            // build attribute map
            var templateAttrs = template.Attributes?.ToDictionary(a => a.Id) ?? new Dictionary<int, TemplateAttributeDto>();

            // validate attributes
            if (dto.Attributes != null)
            {
                foreach (var provided in dto.Attributes)
                {
                    if (!templateAttrs.TryGetValue(provided.AttributeId, out var expected))
                    {
                        throw new ArgumentException($"Attribute id {provided.AttributeId} is not part of template");
                    }

                    var expectedType = (expected.DataType ?? "").ToUpperInvariant();
                    var value = provided.Value;
                    var ok = expectedType switch
                    {
                        "TEXT" => value is string || (value is System.Text.Json.JsonElement je && je.ValueKind == System.Text.Json.JsonValueKind.String),
                        "NUMBER" => ProductServiceHelpers.IsNumber(value),
                        "BOOLEAN" => value is bool || (value is System.Text.Json.JsonElement jeb && (jeb.ValueKind == System.Text.Json.JsonValueKind.True || jeb.ValueKind == System.Text.Json.JsonValueKind.False)),
                        _ => false
                    };

                    if (!ok)
                    {
                        throw new ArgumentException($"Attribute {expected.Name} expects {expectedType} value");
                    }
                }
            }

            // map and persist
            var product = ProductMapper.ToProduct(dto);
            var created = await _productRepository.CreateAsync(product).ConfigureAwait(false);
            _logger.LogInformation("Created product {ProductId} from template {TemplateId}", created.Id, dto.TemplateId);
            return created;
        }
    }

    internal static class ProductServiceHelpers
    {
        public static bool IsNumber(object? value)
        {
            if (value == null) return false;
            return value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal || (value is System.Text.Json.JsonElement je && je.ValueKind == System.Text.Json.JsonValueKind.Number);
        }
    }
}

