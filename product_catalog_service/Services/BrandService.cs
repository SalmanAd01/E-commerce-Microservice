using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using product_catalog_service.Dtos.Brand;
using product_catalog_service.Mappers;
using product_catalog_service.Repositories;

namespace product_catalog_service.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;

        public BrandService(IBrandRepository brandRepository)
        {
            _brandRepository = brandRepository ?? throw new ArgumentNullException(nameof(brandRepository));
        }

        public async Task<BrandDto> CreateBrandAsync(CreateBrandDto createBrandDto)
        {
            var brand = BrandMappers.ToBrandFromCreateBrandDto(createBrandDto);
            var created = await _brandRepository.CreateAsync(brand).ConfigureAwait(false);
            return BrandMappers.ToBrandDto(created);
        }

        public async Task DeleteBrandAsync(string id)
        {
            await _brandRepository.DeleteAsync(id).ConfigureAwait(false);
        }

        public async Task<List<BrandDto>> GetAllBrandsAsync()
        {
            var brands = await _brandRepository.GetAllAsync().ConfigureAwait(false);
            return brands.Select(BrandMappers.ToBrandDto).ToList();
        }

        public async Task<BrandDto?> GetBrandByIdAsync(string id)
        {
            var brand = await _brandRepository.GetByIdAsync(id).ConfigureAwait(false);
            return brand == null ? null : BrandMappers.ToBrandDto(brand);
        }

        public async Task<BrandDto> UpdateBrandAsync(string id, UpdateBrandDto updateBrandDto)
        {
            var brand = BrandMappers.ToBrandFromUpdateBrandDto(updateBrandDto);
            brand.Id = id;
            var updatedEntity = await _brandRepository.UpdateAsync(brand).ConfigureAwait(false);
            if (updatedEntity == null)
            {
                throw new KeyNotFoundException($"Brand with id '{id}' was not found.");
            }

            return BrandMappers.ToBrandDto(updatedEntity);
        }
    }
}