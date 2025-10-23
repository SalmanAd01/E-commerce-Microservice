using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_service.Data;
using inventory_service.Models;
using Microsoft.EntityFrameworkCore;

namespace inventory_service.Repositories
{
    public class StoreRepository: BaseRepository<Store>, IStoreRepository
    {
        public StoreRepository(ApplicationDbContext context) : base(context, "Stores")
        {
        }
    }
}