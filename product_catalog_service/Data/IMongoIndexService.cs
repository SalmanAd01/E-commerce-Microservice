using System.Threading.Tasks;

namespace product_catalog_service.Data
{
    public interface IMongoIndexService
    {
        Task EnsureIndexesAsync();
    }
}
