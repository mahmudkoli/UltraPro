using System;
using System.Collections.Generic;
using UltraPro.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace UltraPro.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public EnumApplicationRoleStatus Status { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationRole() : base()
        {
            this.IsActive = true;
            this.IsDeleted = false;
            this.UserRoles = new List<ApplicationUserRole>();
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
            this.IsActive = true;
            this.IsDeleted = false;
            this.UserRoles = new List<ApplicationUserRole>();
        }
    }
}