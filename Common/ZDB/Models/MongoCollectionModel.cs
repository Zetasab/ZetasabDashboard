namespace ZetaDashboard.Common.ZDB.Models
{
    public class MongoCollectionModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public int Size { get; set; }
        public int StorageSize { get; set; }

        public MongoCollectionModel()
        {

        }
    }
}
