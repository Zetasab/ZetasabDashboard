using MongoDB.Driver;
using ZetaDashboard.Common.Mongo.Config;

namespace ZetaDashboard.Common.Mongo.DataModels
{
    public class MongoContext
    {
        public IMongoDatabase Database { get; }

        public MongoContext(MongoConfig settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            Database = client.GetDatabase(settings.DatabaseName);
        }
    }
}
