using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Xml.Linq;
using ZetaDashboard.Common.Mongo.Config;
using ZetaDashboard.Common.ZDB.Models;

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

        public async Task<List<MongoCollectionModel>> GetCollectionInfoAsync()
        {
            List<MongoCollectionModel> _list = new List<MongoCollectionModel>();
            var collectionsCursor = await _database.ListCollectionNamesAsync();
            var collectionNames = await collectionsCursor.ToListAsync();

            foreach(var name in collectionNames)
            {
                MongoCollectionModel collection = new MongoCollectionModel();

                var command = new BsonDocument { { "collStats", name } };
                var stats = await _database.RunCommandAsync<BsonDocument>(command);

                var count = stats["count"]; // Número de documentos
                var size = stats["size"];   // Tamaño en bytes (datos)
                var storageSize = stats["storageSize"]; // Tamaño en disco

                collection.Name = name;
                collection.Count = Int32.Parse(count.ToString());
                collection.Size = Int32.Parse(size.ToString());
                collection.StorageSize = Int32.Parse(storageSize.ToString());

                _list.Add(collection);
            }
            return _list;
        }

        public async Task<string> GetBackUpCollectionAsync(string name)
        {
            var collection = _database.GetCollection<BsonDocument>(name);
            var documents = await collection.Find(FilterDefinition<BsonDocument>.Empty).ToListAsync();

            // Serializa la lista de BsonDocuments a JSON
            return documents.ToJson();
        }

        public async Task<bool> DeleteCollection(string name)
        {
            try
            {
                await _database.DropCollectionAsync(name);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
