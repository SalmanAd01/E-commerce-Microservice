using System.Threading.Tasks;

namespace ProductCatalog.Infrastructure.Data
{
    public interface IMongoIndexService
    {
        Task EnsureIndexesAsync();
    }
}
