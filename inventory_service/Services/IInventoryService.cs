using System.Collections.Generic;
using System.Threading.Tasks;
using inventory_service.Dtos.Inventory;

namespace inventory_service.Services
{
    public interface IInventoryService
    {
        Task<List<InventoryResponseDto>> GetAllAsync();
        Task<InventoryResponseDto?> GetByIdAsync(int id);
        Task<InventoryResponseDto> CreateAsync(CreateInventoryDto createDto);
        Task<InventoryResponseDto?> UpdateAsync(int id, UpdateInventoryDto updateDto);
        Task DeleteAsync(int id);
    }
}
