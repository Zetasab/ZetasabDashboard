using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MudBlazor;

namespace ZetaDashboard.Common.ZDB.Models
{
    public class UserModel
    {
        public enum EUserType
        {
            Restricted,
            SuperAdmin
        }

        public enum EUserPermissionType
        {
            None,
            Visor,
            Editor,
            Admin
        }

        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public EUserType UserType { get; set; } = EUserType.Restricted;
        public List<UserPermissions> Permissions { get; set; } = new List<UserPermissions>();

        public string MudCustomTheme { get; set; } = "default";

        public UserModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public class UserPermissions
        {
            public string Code { get; set; }
            public EUserPermissionType UserType { get; set; }
        }
    }
}
