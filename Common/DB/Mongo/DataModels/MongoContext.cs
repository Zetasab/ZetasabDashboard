using MongoDB.Driver;
using RailwayDashboard.Common.DB.Mongo.Config;

namespace RailwayDashboard.Common.DB.Mongo.DataModels
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
