using System;
using Inventory.Application.Dtos.Inventory;
using Inventory.Domain.Entities;

namespace Inventory.Application.Mappers
{
    public static class InventoryMapper
    {
        public static InventoryResponseDto ToDto(Inventory.Domain.Entities.Inventory inventory)
        {
            return new InventoryResponseDto
            {
                Id = inventory.Id,
                StoreId = inventory.StoreId,
                ProductSku = inventory.ProductSku,
                ProductId = inventory.ProductId,
                TotalQuantity = inventory.TotalQuantity,
                ReservedQuantity = inventory.ReservedQuantity,
                AvailableQuantity = inventory.AvailableQuantity,
                SellingPrice = inventory.SellingPrice,
                ActualPrice = inventory.ActualPrice,
                CreatedAt = inventory.CreatedAt,
                UpdatedAt = inventory.UpdatedAt
            };
        }

        public static Inventory.Domain.Entities.Inventory FromCreateDto(CreateInventoryDto dto)
        {
            return new Inventory.Domain.Entities.Inventory
            {
                StoreId = dto.StoreId,
                ProductSku = dto.ProductSku,
                ProductId = dto.ProductId,
                TotalQuantity = dto.TotalQuantity,
                ReservedQuantity = dto.ReservedQuantity,
                SellingPrice = dto.SellingPrice,
                ActualPrice = dto.ActualPrice,
            };
        }

        public static void ApplyUpdateDto(Inventory.Domain.Entities.Inventory inventory, UpdateInventoryDto dto)
        {
            inventory.TotalQuantity = dto.TotalQuantity;
            inventory.ReservedQuantity = dto.ReservedQuantity;
            inventory.SellingPrice = dto.SellingPrice;
            inventory.ActualPrice = dto.ActualPrice;
            inventory.UpdatedAt = DateTime.UtcNow;
        }
    }
}
