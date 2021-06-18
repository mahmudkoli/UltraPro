using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Enums;
using UltraPro.Entities;

namespace UltraPro.Repositories.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            //TODO: Need to update IgnoreQueryFilters
            //builder.HasQueryFilter(x => !x.IsDeleted && x.Status != EnumApplicationUserStatus.SuperAdmin);
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.FullName)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}
