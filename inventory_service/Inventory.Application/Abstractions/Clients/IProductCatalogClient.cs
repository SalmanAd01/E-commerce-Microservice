using System.Threading;
using System.Threading.Tasks;

namespace Inventory.Application.Abstractions.Clients
{
    public interface IProductCatalogClient
    {
        Task<bool> ProductExistsWithSkuAsync(string productId, string sku, CancellationToken cancellationToken = default);
    }
}
