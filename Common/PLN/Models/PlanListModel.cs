namespace ZetaDashboard.Common.PLN.Models
{
    public class PlanListModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string OwnerId { get; set; }

        public List<string> UsersIds { get; set; } = new List<string>();

        public List<PlanModel> Plans { get; set; } = new List<PlanModel>();

        public PlanListModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
