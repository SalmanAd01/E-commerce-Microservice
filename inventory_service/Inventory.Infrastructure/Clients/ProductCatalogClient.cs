using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Application.Abstractions.Clients;
using Microsoft.Extensions.Logging;

namespace Inventory.Infrastructure.Clients
{
    public class ProductCatalogClient : IProductCatalogClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ProductCatalogClient> _logger;

        public ProductCatalogClient(IHttpClientFactory httpClientFactory, ILogger<ProductCatalogClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<bool> ProductExistsWithSkuAsync(string productId, string sku, CancellationToken cancellationToken = default)
        {
            var client = _httpClientFactory.CreateClient("product");
            var url = $"api/v1/products/{productId}"; // base address configured on client
            HttpResponseMessage resp;
            try
            {
                resp = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Product service call failed for productId={productId}", productId);
                throw new InvalidOperationException("Failed to call product service", ex);
            }

            if (!resp.IsSuccessStatusCode)
            {
                return false;
            }

            var product = await resp.Content.ReadFromJsonAsync<ProductApiModel?>(cancellationToken: cancellationToken).ConfigureAwait(false);
            if (product == null) return false;
            var variantMatch = product.Variants?.Any(v => string.Equals(v.Sku, sku, StringComparison.OrdinalIgnoreCase)) ?? false;
            return variantMatch;
        }

        private class ProductApiModel
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public VariantModel[]? Variants { get; set; }
        }

        private class VariantModel
        {
            public string? VariantId { get; set; }
            public string? Name { get; set; }
            public string? Sku { get; set; }
        }
    }
}
