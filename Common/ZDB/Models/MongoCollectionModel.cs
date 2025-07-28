namespace ZetaDashboard.Common.ZDB.Models
{
    public class MongoCollectionModel
    {
        public string Name { get; set; }
        public double Count { get; set; }
        public double Size { get; set; }
        public double StorageSize { get; set; }

        public MongoCollectionModel()
        {

        }
    }
}
