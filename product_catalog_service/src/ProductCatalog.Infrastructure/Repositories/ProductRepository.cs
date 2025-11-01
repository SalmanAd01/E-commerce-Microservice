using ProductCatalog.Application.Abstractions.Repositories;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(IMongoDbContext context) : base(context, "Products")
        {
        }
    }
}
