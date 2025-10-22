using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using product_catalog_service.Dtos.Brand;
using product_catalog_service.Mappers;
using product_catalog_service.Repositories;

namespace product_catalog_service.Services
{
    public class BrandService(IBrandRepository brandRepository) : IBrandService
    {
        private readonly IBrandRepository _brandRepository = brandRepository;

        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var brand = BrandMappers.ToBrandFromCreateBrandDto(createBrandDto);
            await _brandRepository.CreateAsync(brand);
            return BrandMappers.ToBrandDto(brand);
        }

        public Task DeleteBrandAsync(string id)
        {
            _brandRepository.DeleteAsync(id);
            return Task.CompletedTask;
        }

        public Task<List<BrandDto>> GetAllBrandsAsync()
        {
            var brands = _brandRepository.GetAllAsync().Result;
            return Task.FromResult(brands.Select(BrandMappers.ToBrandDto).ToList());
        }

        public Task<BrandDto?> GetBrandByIdAsync(string id)
        {
            var brand = _brandRepository.GetByIdAsync(id).Result;
            if (brand == null)
            {
                return Task.FromResult<BrandDto?>(null);
            }
            return Task.FromResult(BrandMappers.ToBrandDto(brand))!;
        }

        public Task UpdateBrandAsync(string id, UpdateBrandDto updateBrandDto)
        {
            var brand = BrandMappers.ToBrandFromUpdateBrandDto(updateBrandDto);
            brand.Id = id;
            return _brandRepository.UpdateAsync(brand);
        }
    }
}