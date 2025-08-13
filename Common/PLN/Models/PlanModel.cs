namespace ZetaDashboard.Common.PLN.Models
{
    public class PlanModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Img { get; set; }
        public PlanStatus Status { get; set; } = PlanStatus.Pending;
        public DateTime? Date { get; set; } = DateTime.Today;
        public string Place { get; set; }
        public PlanModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
    public enum PlanStatus
    {
        Pending,
        Planned,
        Done,
        Cancelled
    }
}
