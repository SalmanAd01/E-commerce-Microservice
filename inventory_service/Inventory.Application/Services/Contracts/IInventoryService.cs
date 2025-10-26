using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inventory.Application.Dtos.Inventory;

namespace Inventory.Application.Services.Contracts
{
    public interface IInventoryService
    {
        Task<List<InventoryResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<InventoryResponseDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<InventoryResponseDto> CreateAsync(CreateInventoryDto createDto, CancellationToken cancellationToken = default);
        Task<InventoryResponseDto?> UpdateAsync(int id, UpdateInventoryDto updateDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<InventoryResponseDto?> GetByStoreAndProductSkuAsync(int storeId, string productSku, CancellationToken cancellationToken = default);
    }
}
