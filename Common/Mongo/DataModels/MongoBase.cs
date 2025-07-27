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
            public async Task UpdateAsync(T entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty == null)
                    throw new InvalidOperationException("Model does not contain an 'Id' property");

                var idValue = idProperty.GetValue(entity);
                if (idValue == null)
                    throw new InvalidOperationException("Entity ID is null.");

                var filter = Builders<T>.Filter.Eq("Id", idValue);
                await _collection.ReplaceOneAsync(filter, entity);
            }

            public async Task DeleteAsync(FilterDefinition<T> filter)
            {
                await _collection.DeleteOneAsync(filter);
            }
            public async Task DeleteAsync(T entity)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                var idProperty = typeof(T).GetProperty("Id");
                if (idProperty == null)
                    throw new InvalidOperationException("El modelo no tiene una propiedad 'Id'.");

                var idValue = idProperty.GetValue(entity);
                if (idValue == null)
                    throw new InvalidOperationException("El valor de 'Id' es null.");

                var filter = Builders<T>.Filter.Eq("Id", idValue);

                var result = await _collection.DeleteOneAsync(filter);

                if (result.DeletedCount == 0)
                    throw new InvalidOperationException("No se encontró el documento para eliminar.");
            }
        }
    }
}
