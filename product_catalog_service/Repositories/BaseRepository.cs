using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using product_catalog_service.Data.Interfaces;

namespace product_catalog_service.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public BaseRepository(IMongoDbContext context, string collectionName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _collection = context.GetCollection<T>(collectionName ?? throw new ArgumentNullException(nameof(collectionName)));
        }

        public async Task<T> CreateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await _collection.InsertOneAsync(entity).ConfigureAwait(false);
            return entity;
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id must be provided", nameof(id));
            var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.BsonValue.Create(id)) |
                         Builders<T>.Filter.Eq("Id", id);
            await _collection.DeleteOneAsync(filter).ConfigureAwait(false);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.BsonValue.Create(id)) |
                         Builders<T>.Filter.Eq("Id", id);
            return await _collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<T?> UpdateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var idProperty = typeof(T).GetProperty("Id") ?? throw new ArgumentException("Entity must have an Id property");
            var idValue = idProperty.GetValue(entity)?.ToString();
            if (string.IsNullOrWhiteSpace(idValue)) throw new ArgumentException("Entity Id property cannot be null or empty");
            var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.BsonValue.Create(idValue)) |
                         Builders<T>.Filter.Eq("Id", idValue);

            var options = new FindOneAndReplaceOptions<T>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = false
            };

            var replaced = await _collection.FindOneAndReplaceAsync(filter, entity, options).ConfigureAwait(false);
            return replaced;
        }
    }
}