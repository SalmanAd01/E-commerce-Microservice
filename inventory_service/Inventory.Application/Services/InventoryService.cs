using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Application.Abstractions.Clients;
using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Dtos.Inventory;
using Inventory.Application.Mappers;
using Microsoft.Extensions.Logging;

namespace Inventory.Application.Services
{
    public class InventoryService : Contracts.IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductCatalogClient _productClient;
        private readonly ILogger<InventoryService> _logger;

        public InventoryService(IInventoryRepository inventoryRepository, IProductCatalogClient productClient, ILogger<InventoryService> logger)
        {
            _inventoryRepository = inventoryRepository;
            _productClient = productClient;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<InventoryResponseDto> CreateAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default)
        {
            // Validate product exists and SKU via Product Catalog client
            var ok = await _productClient.ProductExistsWithSkuAsync(createDto.ProductId, createDto.ProductSku, cancellationToken).ConfigureAwait(false);
            if (!ok) throw new InvalidOperationException("Provided SKU not found in product variants or product not found");

            var existing = await _inventoryRepository.GetByStoreAndSkuAsync(createDto.StoreId, createDto.ProductSku, cancellationToken).ConfigureAwait(false);
            if (existing != null)
            {
                throw new InvalidOperationException("Inventory for this store and SKU already exists");
            }

            var inventory = InventoryMapper.FromCreateDto(createDto);
            var created = await _inventoryRepository.CreateAsync(inventory, cancellationToken).ConfigureAwait(false);
            return InventoryMapper.ToDto(created);
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            await _inventoryRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<InventoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var list = await _inventoryRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
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
            var item = await _inventoryRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (item == null) return null;

            // Validate product exists and SKU using ProductId from inventory model
            var ok = await _productClient.ProductExistsWithSkuAsync(item.ProductId, item.ProductSku, cancellationToken).ConfigureAwait(false);
            if (!ok) throw new InvalidOperationException("Provided SKU not found in product variants or product not found");

            InventoryMapper.ApplyUpdateDto(item, updateDto);
            var updated = await _inventoryRepository.UpdateAsync(item, cancellationToken).ConfigureAwait(false);
            if (updated == null) return null;
            return InventoryMapper.ToDto(updated);
        }

        public async Task<InventoryResponseDto?> GetByStoreAndProductSkuAsync(int storeId, string productSku, CancellationToken cancellationToken = default)
        {
            var item = await _inventoryRepository.GetByStoreAndSkuAsync(storeId, productSku, cancellationToken).ConfigureAwait(false);
            if (item == null) return null;
            return InventoryMapper.ToDto(item);
        }
    }
}
