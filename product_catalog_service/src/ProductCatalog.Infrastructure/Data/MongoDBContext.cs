using MongoDB.Driver;
using Microsoft.Extensions.Options;
using ProductCatalog.Infrastructure.Settings;

namespace ProductCatalog.Infrastructure.Data
{
    public class MongoDBContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IOptions<MongoDbSettings> options)
        {
            var settings = options?.Value ?? throw new ArgumentNullException(nameof(options));
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentException("Collection name must be provided", nameof(collectionName));
            return _database.GetCollection<T>(collectionName);
        }
    }
}
