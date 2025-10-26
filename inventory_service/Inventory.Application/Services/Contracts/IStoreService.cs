using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Application.Dtos.Store;

namespace Inventory.Application.Services.Contracts
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
