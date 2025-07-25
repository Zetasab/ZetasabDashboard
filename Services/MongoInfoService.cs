using MongoDB.Driver;
using ZetaDashboard.Common.Mongo.Config;

namespace ZetaDashboard.Services
{
    public class MongoInfoService
    {
        private readonly IMongoDatabase _database;

        public MongoInfoService(MongoConfig config)
        {
            var client = new MongoClient(config.ConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
        }

        public async Task<List<string>> GetCollectionNamesAsync()
        {
            var collectionsCursor = await _database.ListCollectionNamesAsync();
            var collectionNames = await collectionsCursor.ToListAsync();
            return collectionNames;
        }
    }
}
