using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using product_catalog_service.Attributes;
using product_catalog_service.Data.Interfaces;

namespace product_catalog_service.Data
{
    public class MongoIndexService : IMongoIndexService
    {
        private readonly IMongoDbContext _context;

        public MongoIndexService(IMongoDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task EnsureIndexesAsync()
        {
            var modelTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith("product_catalog_service.Models"))
                .ToList();

            foreach (var type in modelTypes)
            {
                var indexModels = new List<CreateIndexModel<BsonDocument>>();

                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    var attrs = prop.GetCustomAttributes(typeof(MongoIndexAttribute), inherit: true).Cast<MongoIndexAttribute>().ToList();
                    if (!attrs.Any()) continue;

                    var bsonElement = prop.GetCustomAttribute<BsonElementAttribute>();
                    var fieldName = bsonElement?.ElementName ?? prop.Name;

                    foreach (var attr in attrs)
                    {
                        var keys = Builders<BsonDocument>.IndexKeys.Ascending(fieldName);
                        var options = new CreateIndexOptions { Unique = attr.Unique };
                        if (!string.IsNullOrWhiteSpace(attr.Name)) options.Name = attr.Name;
                        indexModels.Add(new CreateIndexModel<BsonDocument>(keys, options));
                    }
                }

                if (!indexModels.Any()) continue;

                var collectionName = type.Name + "s";
                var collection = _context.GetCollection<BsonDocument>(collectionName);
                try
                {
                    await collection.Indexes.CreateManyAsync(indexModels).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Failed to create indexes for collection {collectionName}: {ex.Message}");
                }
            }
        }
    }
}
