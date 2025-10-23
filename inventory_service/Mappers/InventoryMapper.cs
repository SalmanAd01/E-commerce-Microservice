using System;
using inventory_service.Dtos.Inventory;
using inventory_service.Models;

namespace inventory_service.Mappers
{
    public static class InventoryMapper
    {
        public static InventoryResponseDto ToDto(Inventory inventory)
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

        public static Inventory FromCreateDto(CreateInventoryDto dto)
        {
            return new Inventory
            {
                StoreId = dto.StoreId,
                ProductSku = dto.ProductSku,
                // ProductId is a string (product service uses string IDs)
                ProductId = dto.ProductId,
                TotalQuantity = dto.TotalQuantity,
                ReservedQuantity = dto.ReservedQuantity,
                SellingPrice = dto.SellingPrice,
                ActualPrice = dto.ActualPrice,
            };
        }

        public static void ApplyUpdateDto(Inventory inventory, UpdateInventoryDto dto)
        {
            inventory.TotalQuantity = dto.TotalQuantity;
            inventory.ReservedQuantity = dto.ReservedQuantity;
            inventory.SellingPrice = dto.SellingPrice;
            inventory.ActualPrice = dto.ActualPrice;
            inventory.UpdatedAt = DateTime.UtcNow;
        }
    }
}
