
namespace UltraPro.API.Models.IdentityModels
{
    public static class Permissions
    {
        public const string SuperAdmin = "Permissions.SuperAdmin";

        public static class Users
        {
            public const string ListView = "Permissions.Users.ListView";
            public const string DetailsView = "Permissions.Users.DetailsView";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }

        public static class UserRoles
        {
            public const string ListView = "Permissions.UserRoles.ListView";
            public const string DetailsView = "Permissions.UserRoles.DetailsView";
            public const string Create = "Permissions.UserRoles.Create";
            public const string Edit = "Permissions.UserRoles.Edit";
            //public const string CreateOrEdit = "Permissions.UserRoles.CreateOrEdit";
            public const string Delete = "Permissions.UserRoles.Delete";
        }

        public static class SingleValue
        {
            public const string ListView = "Permissions.SingleValue.ListView";
            public const string DetailsView = "Permissions.SingleValue.DetailsView";
            public const string Create = "Permissions.SingleValue.Create";
            public const string Edit = "Permissions.SingleValue.Edit";
            //public const string CreateOrEdit = "Permissions.SingleValue.CreateOrEdit";
            public const string Delete = "Permissions.SingleValue.Delete";
        }

        public static class RequestResponseLogs
        {
            public const string ListView = "Permissions.RequestResponseLog.ListView";
            public const string DetailsView = "Permissions.RequestResponseLog.DetailsView";
        }

        public static class AuditLogs
        {
            public const string ListView = "Permissions.AuditLog.ListView";
            public const string DetailsView = "Permissions.AuditLog.DetailsView";
        }
    }
}