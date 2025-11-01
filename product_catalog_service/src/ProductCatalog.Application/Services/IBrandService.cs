using System.Collections.Generic;
using System.Threading.Tasks;
using ProductCatalog.Application.Dtos.Brand;

namespace ProductCatalog.Application.Services
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
