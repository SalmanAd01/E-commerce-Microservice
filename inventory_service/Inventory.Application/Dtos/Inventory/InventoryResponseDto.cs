using System;

namespace Inventory.Application.Dtos.Inventory
{
    public class InventoryResponseDto
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public string ProductSku { get; set; } = null!;
        public string ProductId { get; set; } = null!;
        public int TotalQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal ActualPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
