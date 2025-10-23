using System.ComponentModel.DataAnnotations;

namespace inventory_service.Dtos.Inventory
{
    public class CreateInventoryDto : IValidatableObject
    {
        [Required]
        public int StoreId { get; set; }

        [Required]
        [MaxLength(50)]
        public string ProductSku { get; set; } = null!;

        [Required]
        public string ProductId { get; set; } = null!; // string because external API uses string ids

        [Required]
        [Range(0, int.MaxValue)]
        public int TotalQuantity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ReservedQuantity { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        public decimal ActualPrice { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReservedQuantity > TotalQuantity)
            {
                yield return new ValidationResult("reserved_quantity must be less than or equal to total_quantity", new[] { nameof(ReservedQuantity), nameof(TotalQuantity) });
            }

            if (SellingPrice > ActualPrice)
            {
                yield return new ValidationResult("selling_price must be less than or equal to actual_price", new[] { nameof(SellingPrice), nameof(ActualPrice) });
            }
        }
    }
}
