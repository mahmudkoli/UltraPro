using System;
using System.Collections.Generic;
using UltraPro.Common.Enums;
using Microsoft.AspNetCore.Identity;

namespace UltraPro.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string ImageUrl { get; set; }

        public string LastPassword { get; set; }
        public DateTime? LastPassChangeDate { get; set; }
        public int PasswordChangedCount { get; set; }
        public EnumApplicationUserStatus Status { get; set; }

        public Guid? CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationUser() : base()
        {
            this.IsActive = true;
            this.IsDeleted = false;
            this.UserRoles = new List<ApplicationUserRole>();
        }
    }
}