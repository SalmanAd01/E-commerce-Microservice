using ProductCatalog.Application.Dtos.Brand;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Mappers
{
    public static class BrandMappers
    {
        public static Brand ToBrandFromCreateBrandDto(CreateBrandDto dto)
        {
            return new Brand
            {
                Name = dto.Name,
                Description = dto.Description,
                Logo = dto.Logo
            };
        }

        public static BrandDto ToBrandDto(Brand brand)
        {
            return new BrandDto
            {
                Id = brand.Id ?? string.Empty,
                Name = brand.Name,
                Description = brand.Description,
                Logo = brand.Logo
            };
        }

        public static Brand ToBrandFromUpdateBrandDto(UpdateBrandDto dto)
        {
            return new Brand
            {
                Name = dto.Name,
                Description = dto.Description,
                Logo = dto.Logo
            };
        }
    }
}
