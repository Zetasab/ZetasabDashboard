using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RailwayDashboard.Common.Models
{
    public class User
    {
        public enum EUserType
        {
            Normal,
            //Editor,
            Admin,
            SuperAdmin
        }
        
        public string Id { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public EUserType UserType { get; set; }
        public List<string> Permissions { get; set; }


        public User()
        {
            Id = Guid.NewGuid().ToString();
        }

    }
}
