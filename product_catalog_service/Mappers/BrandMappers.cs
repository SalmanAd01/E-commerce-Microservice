using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using product_catalog_service.Dtos.Brand;
using product_catalog_service.Models;

namespace product_catalog_service.Mappers
{
    public class BrandMappers
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
                Id = brand.Id,
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