using System.Threading.Tasks;
using Inventory.Domain.Entities;

namespace Inventory.Application.Abstractions.Repositories
{
    public interface IInventoryRepository : IBaseRepository<Inventory.Domain.Entities.Inventory>
    {
        Task<Inventory.Domain.Entities.Inventory?> GetByStoreAndSkuAsync(int storeId, string sku, System.Threading.CancellationToken cancellationToken = default);
        Task<bool> TryReserveAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default);
        Task ReleaseReservationAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default);
        Task<bool> CommitReservationAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default);
    }
}
