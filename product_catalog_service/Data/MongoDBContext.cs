using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using product_catalog_service.Data.Interfaces;
using product_catalog_service.Settings;

namespace product_catalog_service.Data
{
    public class MongoDBContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(Microsoft.Extensions.Options.IOptions<MongoDbSettings> options)
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