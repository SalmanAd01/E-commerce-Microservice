using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Application.Abstractions.Repositories;
using Inventory.Application.Dtos.Store;
using Inventory.Application.Mappers;

namespace Inventory.Application.Services
{
    public class StoreService : Contracts.IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
        }

        public async Task<StoreResponseDto> CreateStoreAsync(CreateStoreDto createDto, CancellationToken cancellationToken = default)
        {
            var store = StoreMapper.FromCreateDto(createDto);
            var createdStore = await _storeRepository.CreateAsync(store, cancellationToken).ConfigureAwait(false);
            return StoreMapper.ToDto(createdStore);
        }

        public async Task DeleteStoreAsync(int id, CancellationToken cancellationToken = default)
        {
            await _storeRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<List<StoreResponseDto>> GetAllStoresAsync(CancellationToken cancellationToken = default)
        {
            var stores = await _storeRepository.GetAllAsync(cancellationToken).ConfigureAwait(false);
            return stores.Where(s => s != null).Select(s => StoreMapper.ToDto(s!)).ToList();
        }

        public async Task<StoreResponseDto?> GetStoreByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var store = await _storeRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (store == null) return null;
            return StoreMapper.ToDto(store);
        }

        public async Task<StoreResponseDto?> UpdateStoreAsync(int id, UpdateStoreDto updateDto, CancellationToken cancellationToken = default)
        {
            var store = await _storeRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (store == null) return null;

            store.Name = updateDto.Name;
            store.StoreCode = updateDto.StoreCode;
            store.Address = updateDto.Address;
            store.City = updateDto.City;
            store.State = updateDto.State;
            store.ZipCode = updateDto.ZipCode;
            store.Country = updateDto.Country;

            var updatedStore = await _storeRepository.UpdateAsync(store, cancellationToken).ConfigureAwait(false);
            if (updatedStore == null) return null;
            return StoreMapper.ToDto(updatedStore);
        }
    }
}
