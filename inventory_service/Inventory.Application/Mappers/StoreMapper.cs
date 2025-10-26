using System;
using Inventory.Application.Dtos.Store;
using Inventory.Domain.Entities;

namespace Inventory.Application.Mappers
{
    public static class StoreMapper
    {
        public static StoreResponseDto ToDto(Store store)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            return new StoreResponseDto
            {
                Id = store.Id,
                Name = store.Name,
                StoreCode = store.StoreCode,
                Address = store.Address,
                City = store.City,
                State = store.State,
                ZipCode = store.ZipCode,
                Country = store.Country,
                CreatedAt = store.CreatedAt,
                UpdatedAt = store.UpdatedAt
            };
        }

        public static Store FromCreateDto(CreateStoreDto createDto)
        {
            if (createDto == null) throw new ArgumentNullException(nameof(createDto));
            return new Store
            {
                Name = createDto.Name,
                StoreCode = createDto.StoreCode,
                Address = createDto.Address,
                City = createDto.City,
                State = createDto.State,
                ZipCode = createDto.ZipCode,
                Country = createDto.Country
            };
        }
    }
}
