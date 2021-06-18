using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Constants;
using UltraPro.Common.Enums;
using UltraPro.Entities;

namespace UltraPro.Repositories.Configurations
{
    public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            //builder.HasQueryFilter(x => !x.IsDeleted && x.Status != EnumApplicationRoleStatus.SuperAdmin);
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(30);
        }
    }
}
