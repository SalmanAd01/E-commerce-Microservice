using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using product_catalog_service.Dtos.Product;
using product_catalog_service.Models;

namespace product_catalog_service.Services
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
