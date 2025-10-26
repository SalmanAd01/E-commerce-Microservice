using FluentValidation;
using Inventory.Application.Dtos.Store;

namespace Inventory.Application.Validation
{
    public class UpdateStoreDtoValidator : AbstractValidator<UpdateStoreDto>
    {
        public UpdateStoreDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.StoreCode).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Address).NotEmpty().MaximumLength(200);
            RuleFor(x => x.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.State).NotEmpty().MaximumLength(100);
            RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        }
    }
}
