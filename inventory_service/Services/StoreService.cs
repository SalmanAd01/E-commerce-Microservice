using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Dtos.Store;
using inventory_service.Exceptions;
using inventory_service.Mappers;
using inventory_service.Repositories;

namespace inventory_service.Services
{
    public class StoreService : IStoreService
    {
        private readonly IStoreRepository _storeRepository;
        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }
        public async Task<StoreResponseDto> CreateStoreAsync(CreateStoreDto createDto)
        {
            var store = StoreMapper.FromCreateDto(createDto);
                var createdStore = await _storeRepository.CreateAsync(store);
                return StoreMapper.ToDto(createdStore);
        }

        public async Task DeleteStoreAsync(int id)
        {
            await _storeRepository.DeleteAsync(id);
        }

        public async Task<List<StoreResponseDto>> GetAllStoresAsync()
        {
            var stores = await _storeRepository.GetAllAsync();
            // Some repositories might return nullable items; filter them out before mapping.
            return stores.Where(s => s != null).Select(s => StoreMapper.ToDto(s!)).ToList();
        }

        public async Task<StoreResponseDto?> GetStoreByIdAsync(int id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null) return null;
            return StoreMapper.ToDto(store);
        }

        public async Task<StoreResponseDto?> UpdateStoreAsync(int id, UpdateStoreDto updateDto)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null)
            {
                return null;
            }

            store.Name = updateDto.Name;
            store.StoreCode = updateDto.StoreCode;
            store.Address = updateDto.Address;
            store.City = updateDto.City;
            store.State = updateDto.State;
            store.ZipCode = updateDto.ZipCode;
            store.Country = updateDto.Country;

            var updatedStore = await _storeRepository.UpdateAsync(store);
            if (updatedStore == null) return null;
            return StoreMapper.ToDto(updatedStore);
        }
    }
}