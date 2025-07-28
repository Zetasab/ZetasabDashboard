using ZetaDashboard.Common.Mongo;
using static MudBlazor.CategoryTypes;

namespace ZetaDashboard.Common.ZDB.Models
{
    public enum AuditWhat
    {
        Login,
        See,
        Post,
        Put,
        Delete,
        BackUp
    }
    public class AuditModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public AuditWhat What { get; set; }
        public string Where { get; set; }
        public string Description { get; set; }
        public string Details { get; set; }
        public DateTime When { get; set; } = DateTime.UtcNow;

        public ResponseStatus Status { get; set; }

        public AuditModel()
        {
            Id = Guid.NewGuid().ToString();
        }
        public AuditModel(
                string userId,
                string userName,
                AuditWhat what,
                string where,
                string description,
                ResponseStatus status,
                string details = "")
        {
            Id = Guid.NewGuid().ToString();
            UserId = userId;
            UserName = userName;
            What = what;
            Where = where;
            Description = description;
            When = DateTime.Now;
            Status = status;
            Details = details;
        }
    }
}
