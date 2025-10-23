using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Dtos.Store;

namespace inventory_service.Services
{
    public interface IStoreService
    {
        Task<List<StoreResponseDto>> GetAllStoresAsync();
        Task<StoreResponseDto?> GetStoreByIdAsync(int id);
        Task<StoreResponseDto> CreateStoreAsync(CreateStoreDto createDto);
        Task<StoreResponseDto?> UpdateStoreAsync(int id, UpdateStoreDto updateDto);
        Task DeleteStoreAsync(int id);
    }
}