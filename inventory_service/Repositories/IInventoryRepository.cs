using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Models;

namespace inventory_service.Repositories
{
    public interface IInventoryRepository: IBaseRepository<Inventory>
    {
        Task<Inventory?> GetByStoreAndSkuAsync(int storeId, string sku);
    }
}
