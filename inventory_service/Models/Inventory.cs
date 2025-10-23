using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace inventory_service.Models
{
    [Index(nameof(StoreId), nameof(ProductSku), IsUnique = true)]
    [Table("inventories")]
    public class Inventory
    {
        [Column("id")]
        [Key]
        public int Id { get; set; }

        [Column("store_id")]
        [Required]
        public int StoreId { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; } = null!;

        [Column("product_sku")]
        [MaxLength(50)]
        [Required]
        public required string ProductSku { get; set; }

        [Column("product_id")]
        [Required]
        public required string ProductId { get; set; }

        [Column("total_quantity")]
        [Required]
        public int TotalQuantity { get; set; }

        [Column("reserved_quantity")]
        [Required]
        public int ReservedQuantity { get; set; }

        [NotMapped]
        public int AvailableQuantity => TotalQuantity - ReservedQuantity;

        [Column("selling_price")]
        [Required]
        public decimal SellingPrice { get; set; }

        [Column("actual_price")]
        [Required]
        public decimal ActualPrice { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
