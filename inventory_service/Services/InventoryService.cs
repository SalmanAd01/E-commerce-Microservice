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
        public InventoryService(IInventoryRepository inventoryRepository, IHttpClientFactory httpClientFactory)
        {
            _inventoryRepository = inventoryRepository;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<InventoryResponseDto> CreateAsync(CreateInventoryDto createDto)
        {
            // Validate product exists and SKU
            await ValidateProductAndSku(createDto.ProductId, createDto.ProductSku);

            var existing = await _inventoryRepository.GetByStoreAndSkuAsync(createDto.StoreId, createDto.ProductSku);
            if (existing != null)
            {
                throw new InvalidOperationException("Inventory for this store and SKU already exists");
            }

            var inventory = InventoryMapper.FromCreateDto(createDto);
            var created = await _inventoryRepository.CreateAsync(inventory);
            return InventoryMapper.ToDto(created);
        }

        public async Task DeleteAsync(int id)
        {
            await _inventoryRepository.DeleteAsync(id);
        }

        public async Task<List<InventoryResponseDto>> GetAllAsync()
        {
            var list = await _inventoryRepository.GetAllAsync();
            return list.Where(i => i != null).Select(i => InventoryMapper.ToDto(i!)).ToList();
        }

        public async Task<InventoryResponseDto?> GetByIdAsync(int id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null) return null;
            return InventoryMapper.ToDto(item);
        }

        public async Task<InventoryResponseDto?> UpdateAsync(int id, UpdateInventoryDto updateDto)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null) return null;

            // Validate product exists and SKU using ProductId from inventory model
            await ValidateProductAndSku(item.ProductId, item.ProductSku);

            InventoryMapper.ApplyUpdateDto(item, updateDto);
            var updated = await _inventoryRepository.UpdateAsync(item);
            if (updated == null) return null;
            return InventoryMapper.ToDto(updated);
        }

        private async Task ValidateProductAndSku(string productId, string sku)
        {
            // Call external product service
            var client = _httpClientFactory.CreateClient();
            var url = $"http://localhost:5234/api/v1/products/{productId}";
            HttpResponseMessage resp;
            try
            {
                resp = await client.GetAsync(url);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to call product service", ex);
            }

            if (!resp.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Product not found or product service returned an error");
            }

            var product = await resp.Content.ReadFromJsonAsync<ProductApiModel?>();
            if (product == null) throw new InvalidOperationException("Invalid product response");

            // Check SKU exists in variants
            var variantMatch = product.Variants?.Any(v => string.Equals(v.Sku, sku, StringComparison.OrdinalIgnoreCase)) ?? false;
            if (!variantMatch)
            {
                throw new InvalidOperationException("Provided SKU not found in product variants");
            }
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
