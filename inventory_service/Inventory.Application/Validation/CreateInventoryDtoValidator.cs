using FluentValidation;
using Inventory.Application.Dtos.Inventory;

namespace Inventory.Application.Validation
{
    public class CreateInventoryDtoValidator : AbstractValidator<CreateInventoryDto>
    {
        public CreateInventoryDtoValidator()
        {
            RuleFor(x => x.StoreId).GreaterThan(0);
            RuleFor(x => x.ProductSku).NotEmpty().MaximumLength(50);
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.TotalQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ReservedQuantity).GreaterThanOrEqualTo(0);
            RuleFor(x => x.SellingPrice).GreaterThanOrEqualTo(0);
            RuleFor(x => x.ActualPrice).GreaterThanOrEqualTo(0);

            RuleFor(x => x.ReservedQuantity)
                .LessThanOrEqualTo(x => x.TotalQuantity)
                .WithMessage("reserved_quantity must be less than or equal to total_quantity");
            RuleFor(x => x.SellingPrice)
                .LessThanOrEqualTo(x => x.ActualPrice)
                .WithMessage("selling_price must be less than or equal to actual_price");
        }
    }
}
