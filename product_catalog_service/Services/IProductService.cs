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
    }
}
