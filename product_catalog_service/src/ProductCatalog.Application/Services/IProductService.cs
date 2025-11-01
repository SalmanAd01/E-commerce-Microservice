using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ProductCatalog.Application.Dtos.Product;
using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Application.Services
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken = default);
        Task<List<Product>> GetAllProductsAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Product> UpdateProductAsync(string id, CreateProductDto dto, CancellationToken cancellationToken = default);
        Task DeleteProductAsync(string id, CancellationToken cancellationToken = default);
    }
}
