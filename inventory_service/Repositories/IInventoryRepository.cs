using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Models;

namespace inventory_service.Repositories
{
    public interface IInventoryRepository: IBaseRepository<Inventory>
    {
        Task<Inventory?> GetByStoreAndSkuAsync(int storeId, string sku, System.Threading.CancellationToken cancellationToken = default);
        /// <summary>
        /// Try to reserve the given quantity for the store/sku atomically. Returns true if reservation succeeded.
        /// </summary>
        Task<bool> TryReserveAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default);

        /// <summary>
        /// Release a previously reserved quantity (decrements reserved_quantity). Quantity will be clamped to available reserved amount.
        /// </summary>
        Task ReleaseReservationAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default);
    }
}
