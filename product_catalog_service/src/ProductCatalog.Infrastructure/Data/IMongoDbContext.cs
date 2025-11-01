using MongoDB.Driver;

namespace ProductCatalog.Infrastructure.Data
{
    public interface IMongoDbContext
    {
        IMongoCollection<T> GetCollection<T>(string collectionName);
    }
}
