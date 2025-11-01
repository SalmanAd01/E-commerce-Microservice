using MongoDB.Driver;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories
{
    public class BaseRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> Collection;

        public BaseRepository(IMongoDbContext context, string collectionName)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            Collection = context.GetCollection<T>(collectionName ?? throw new ArgumentNullException(nameof(collectionName)));
        }

        public async Task<T> CreateAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            await Collection.InsertOneAsync(entity).ConfigureAwait(false);
            return entity;
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("id must be provided", nameof(id));
            var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.BsonValue.Create(id)) |
                         Builders<T>.Filter.Eq("Id", id);
            await Collection.DeleteOneAsync(filter).ConfigureAwait(false);
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await Collection.Find(_ => true).ToListAsync().ConfigureAwait(false);
        }

        public async Task<T?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            var filter = Builders<T>.Filter.Eq("_id", MongoDB.Bson.BsonValue.Create(id)) |
                         Builders<T>.Filter.Eq("Id", id);
            return await Collection.Find(filter).FirstOrDefaultAsync().ConfigureAwait(false);
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

            var replaced = await Collection.FindOneAndReplaceAsync(filter, entity, options).ConfigureAwait(false);
            return replaced;
        }
    }
}
