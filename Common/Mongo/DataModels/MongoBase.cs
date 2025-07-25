using MongoDB.Driver;

namespace ZetaDashboard.Common.Mongo.DataModels
{
    public class MongoBase
    {
        public abstract class MongoRepositoryBase<T>
        {
            
            protected readonly IMongoCollection<T> _collection;

            protected MongoRepositoryBase(MongoContext context, string collectionName)
            {
                _collection = context.Database.GetCollection<T>(collectionName);
            }

            public async Task<List<T>> FindAllAsync()
            {
                return await _collection.Find(_ => true).ToListAsync();
            }

            public async Task<T?> FindFirstAsync(FilterDefinition<T> filter)
            {
                return await _collection.Find(filter).FirstOrDefaultAsync();
            }

            public async Task InsertAsync(T entity)
            {
                await _collection.InsertOneAsync(entity);
            }

            public async Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
            {
                await _collection.UpdateOneAsync(filter, update);
            }

            public async Task DeleteAsync(FilterDefinition<T> filter)
            {
                await _collection.DeleteOneAsync(filter);
            }
        }
    }
}
