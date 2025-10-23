using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Dtos.Store;
using inventory_service.Models;

namespace inventory_service.Mappers
{
    public class StoreMapper
    {
        public static StoreResponseDto toDto(Store store)
        {
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

        public static Store fromCreateDto(CreateStoreDto createDto)
        {
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