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

        public async Task<Inventory?> GetByStoreAndSkuAsync(int storeId, string sku)
        {
            return await _context.Inventories.FirstOrDefaultAsync(i => i.StoreId == storeId && i.ProductSku == sku);
        }
    }
}
