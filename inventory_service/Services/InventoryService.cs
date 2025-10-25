using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using inventory_service.Dtos.Inventory;
using inventory_service.Mappers;
using inventory_service.Models;
using inventory_service.Repositories;

namespace inventory_service.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(IInventoryRepository inventoryRepository, IHttpClientFactory httpClientFactory, ILogger<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<InventoryResponseDto> CreateAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default)
        {
            // Validate product exists and SKU
            await ValidateProductAndSku(createDto.ProductId, createDto.ProductSku, cancellationToken).ConfigureAwait(false);

            var existing = await _inventoryRepository.GetByStoreAndSkuAsync(createDto.StoreId, createDto.ProductSku, cancellationToken).ConfigureAwait(false);
            if (existing != null)
            {
                throw new InvalidOperationException("Inventory for this store and SKU already exists");
            }

            var inventory = InventoryMapper.FromCreateDto(createDto);
            var created = await _inventoryRepository.CreateAsync(inventory).ConfigureAwait(false);
            return InventoryMapper.ToDto(created);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _inventoryRepository.DeleteAsync(id).ConfigureAwait(false);
        }

        public async Task<List<InventoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var list = await _inventoryRepository.GetAllAsync().ConfigureAwait(false);
            return list.Where(i => i != null).Select(i => InventoryMapper.ToDto(i!)).ToList();
        }

        public async Task<InventoryResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _inventoryRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (item == null) return null;
            return InventoryMapper.ToDto(item);
        }

        public async Task<InventoryResponseDto?> UpdateAsync(int id, UpdateInventoryDto updateDto, CancellationToken cancellationToken = default)
        {
            var item = await _inventoryRepository.GetByIdAsync(id).ConfigureAwait(false);
            if (item == null) return null;

            // Validate product exists and SKU using ProductId from inventory model
            await ValidateProductAndSku(item.ProductId, item.ProductSku, cancellationToken).ConfigureAwait(false);

            InventoryMapper.ApplyUpdateDto(item, updateDto);
            var updated = await _inventoryRepository.UpdateAsync(item, cancellationToken).ConfigureAwait(false);
            if (updated == null) return null;
            return InventoryMapper.ToDto(updated);
        }

        private async Task ValidateProductAndSku(string productId, string sku, CancellationToken cancellationToken = default)
        {
            // Call external product service
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
                throw new InvalidOperationException("Product not found or product service returned an error");
            }

            var product = await resp.Content.ReadFromJsonAsync<ProductApiModel?>(cancellationToken: cancellationToken).ConfigureAwait(false);
            if (product == null) throw new InvalidOperationException("Invalid product response");

            // Check SKU exists in variants
            var variantMatch = product.Variants?.Any(v => string.Equals(v.Sku, sku, StringComparison.OrdinalIgnoreCase)) ?? false;
            if (!variantMatch)
            {
                throw new InvalidOperationException("Provided SKU not found in product variants");
            }
        }

        public async Task<InventoryResponseDto?> GetByStoreAndProductSkuAsync(int storeId, string productSku, CancellationToken cancellationToken = default)
        {
            var item = await _inventoryRepository.GetByStoreAndSkuAsync(storeId, productSku, cancellationToken).ConfigureAwait(false);
            if (item == null) return null;
            return InventoryMapper.ToDto(item);
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
