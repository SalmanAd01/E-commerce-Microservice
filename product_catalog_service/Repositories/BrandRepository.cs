using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using product_catalog_service.Data.Interfaces;
using product_catalog_service.Models;

namespace product_catalog_service.Repositories
{
    public class BrandRepository(IMongoDbContext context) : BaseRepository<Brand>(context, "Brands"), IBrandRepository
    {
    }
}