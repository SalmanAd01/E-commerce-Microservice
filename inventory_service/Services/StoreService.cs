using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Dtos.Store;
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
            var store = StoreMapper.fromCreateDto(createDto);
            try
            {
                var createdStore = await _storeRepository.CreateAsync(store);
                return StoreMapper.toDto(createdStore);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx) when (dbEx.InnerException != null)
            {
                var inner = dbEx.InnerException;
                var raw = inner.Message ?? string.Empty;
                var match = System.Text.RegularExpressions.Regex.Match(raw, @"Key \(([^)]+)\)=\(([^)]+)\)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var column = match.Groups[1].Value;
                    var value = match.Groups[2].Value;
                    throw new inventory_service.Exceptions.DuplicateKeyException(column, value);
                }
                if (raw.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) >= 0 || raw.IndexOf("unique", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    throw new inventory_service.Exceptions.DuplicateKeyException(raw);
                }

                throw;
            }
        }

        public async Task DeleteStoreAsync(int id)
        {
            await _storeRepository.DeleteAsync(id);
        }

        public async Task<List<StoreResponseDto>> GetAllStoresAsync()
        {
            var stores = await _storeRepository.GetAllAsync();
            // Some repositories might return nullable items; filter them out before mapping.
            return stores.Where(s => s != null).Select(StoreMapper.toDto!).ToList();
        }

        public async Task<StoreResponseDto?> GetStoreByIdAsync(int id)
        {
            var store = await _storeRepository.GetByIdAsync(id);
            if (store == null) return null;
            return StoreMapper.toDto(store);
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
            return StoreMapper.toDto(updatedStore);
        }
    }
}