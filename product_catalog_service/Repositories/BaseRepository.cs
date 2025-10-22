using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using product_catalog_service.Data.Interfaces;

namespace product_catalog_service.Repositories
{
    public class BaseRepository<T>(IMongoDbContext context, string collectionName) : IBaseRepository<T> where T : class
    {
        private readonly IMongoCollection<T> _collection = context.GetCollection<T>(collectionName);

        public async Task<T> CreateAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("Id", id));
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            return await _collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstOrDefaultAsync();  
        }

        public async Task UpdateAsync(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id") ?? throw new ArgumentException("Entity must have an Id property");
            var idValue = (idProperty.GetValue(entity)?.ToString()) ?? throw new ArgumentException("Entity Id property cannot be null");
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", idValue), entity);
        }
    }
}