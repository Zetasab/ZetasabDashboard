namespace ZetaDashboard.Common.ZDB.Models
{
    public class ProyectModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Icon { get; set; }

        public string Url { get; set; }
        public bool IsHere { get; set; } = false;

        public ProyectModel() 
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
