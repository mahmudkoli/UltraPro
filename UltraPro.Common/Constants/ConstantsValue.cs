using System;
using System.Collections.Generic;
using System.Text;

namespace UltraPro.Common.Constants
{
    public class ConstantsApplication
    {
        public const string SerilogMSSqlServerTableName = "ApplicationLogs";
        public const string SerilogMSSqlServerAdditionalColumnUserName = "UserName";
    }

    public class ConstantsUserRole
    {
        public const string SuperAdmin = "SuperAdmin";
        public const string Admin = "Admin";
        public const string GeneralUser = "GeneralUser";
        public const string AppUser = "AppUser";
    }

    public class ConstantsRolePermission
    {
        public const string Type = "Permission";
        public const string Value = "Permissions.SuperAdmin";
    }
}
