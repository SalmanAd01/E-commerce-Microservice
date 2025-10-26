using System.Threading.Tasks;
using Inventory.Application.Abstractions.Repositories;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory.Domain.Entities.Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _context;
        public InventoryRepository(ApplicationDbContext context) : base(context, "Inventories")
        {
            _context = context;
        }

        public async Task<Inventory.Domain.Entities.Inventory?> GetByStoreAndSkuAsync(int storeId, string sku, System.Threading.CancellationToken cancellationToken = default)
        {
            return await _context.Inventories.FirstOrDefaultAsync(i => i.StoreId == storeId && i.ProductSku == sku, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> TryReserveAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default)
        {
            if (quantity <= 0) return false;

            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE inventories SET reserved_quantity = reserved_quantity + {quantity}, updated_at = NOW() WHERE store_id = {storeId} AND product_sku = {sku} AND (total_quantity - reserved_quantity) >= {quantity}", cancellationToken).ConfigureAwait(false);
            return res > 0;
        }

        public async Task ReleaseReservationAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default)
        {
            if (quantity <= 0) return;

            await _context.Database.ExecuteSqlInterpolatedAsync($@"UPDATE inventories
SET reserved_quantity = GREATEST(reserved_quantity - {quantity}, 0),
    total_quantity = total_quantity + {quantity},
    updated_at = NOW()
WHERE store_id = {storeId} AND product_sku = {sku}", cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> CommitReservationAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default)
        {
            if (quantity <= 0) return false;

            // Atomically decrement reserved and total only if there is sufficient reserved and total quantity
            var res = await _context.Database.ExecuteSqlInterpolatedAsync($@"UPDATE inventories
SET reserved_quantity = reserved_quantity - {quantity},
    total_quantity = total_quantity - {quantity},
    updated_at = NOW()
WHERE store_id = {storeId}
  AND product_sku = {sku}
  AND reserved_quantity >= {quantity}
  AND total_quantity >= {quantity}", cancellationToken).ConfigureAwait(false);

            return res > 0;
        }
    }
}
