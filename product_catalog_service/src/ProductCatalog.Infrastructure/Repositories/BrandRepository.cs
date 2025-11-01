using ProductCatalog.Application.Abstractions.Repositories;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class BrandRepository : BaseRepository<Brand>, IBrandRepository
    {
        public BrandRepository(IMongoDbContext context) : base(context, "Brands")
        {
        }
    }
}
