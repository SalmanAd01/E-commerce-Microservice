using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Data;
using inventory_service.Models;
using Microsoft.EntityFrameworkCore;

namespace inventory_service.Repositories
{
    public class InventoryRepository : BaseRepository<Inventory>, IInventoryRepository
    {
        private readonly ApplicationDbContext _context;
        public InventoryRepository(ApplicationDbContext context) : base(context, "Inventories")
        {
            _context = context;
        }

        public async Task<Inventory?> GetByStoreAndSkuAsync(int storeId, string sku, System.Threading.CancellationToken cancellationToken = default)
        {
            return await _context.Inventories.FirstOrDefaultAsync(i => i.StoreId == storeId && i.ProductSku == sku, cancellationToken).ConfigureAwait(false);
        }

        public async Task<bool> TryReserveAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default)
        {
            if (quantity <= 0) return false;

            // Perform an atomic update that only succeeds if available quantity is sufficient.
            var sql = $@"UPDATE inventories
SET reserved_quantity = reserved_quantity + {quantity}, updated_at = NOW()
WHERE store_id = {storeId} AND product_sku = {{0}} AND (total_quantity - reserved_quantity) >= {quantity}";

            // Use ExecuteSqlInterpolatedAsync for parameterization of sku
            var res = await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE inventories SET reserved_quantity = reserved_quantity + {quantity}, updated_at = NOW() WHERE store_id = {storeId} AND product_sku = {sku} AND (total_quantity - reserved_quantity) >= {quantity}", cancellationToken).ConfigureAwait(false);
            return res > 0;
        }

        public async Task ReleaseReservationAsync(int storeId, string sku, int quantity, System.Threading.CancellationToken cancellationToken = default)
        {
            if (quantity <= 0) return;

            // Decrement reserved_quantity but do not go below zero
            // Use SQL to clamp to zero
            await _context.Database.ExecuteSqlInterpolatedAsync($@"UPDATE inventories
SET reserved_quantity = GREATEST(reserved_quantity - {quantity}, 0), updated_at = NOW()
WHERE store_id = {storeId} AND product_sku = {sku}", cancellationToken).ConfigureAwait(false);
        }
    }
}
