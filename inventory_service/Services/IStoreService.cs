using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Dtos.Store;

namespace inventory_service.Services
{
    public interface IStoreService
    {
        Task<List<StoreResponseDto>> GetAllStoresAsync(CancellationToken cancellationToken = default);
        Task<StoreResponseDto?> GetStoreByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<StoreResponseDto> CreateStoreAsync(CreateStoreDto createDto, CancellationToken cancellationToken = default);
        Task<StoreResponseDto?> UpdateStoreAsync(int id, UpdateStoreDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteStoreAsync(int id, CancellationToken cancellationToken = default);
    }
}