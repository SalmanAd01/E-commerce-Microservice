using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using product_catalog_service.Dtos.Brand;

namespace product_catalog_service.Services
{
    public interface IBrandService
    {
        Task<List<BrandDto>> GetAllBrandsAsync();
        Task<BrandDto?> GetBrandByIdAsync(string id);
        Task<BrandDto> CreateBrandAsync(CreateBrandDto createBrandDto);
        Task<BrandDto> UpdateBrandAsync(string id, UpdateBrandDto updateBrandDto);
        Task DeleteBrandAsync(string id);
    }
}