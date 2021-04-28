using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;

namespace UltraPro.Repositories.Configurations
{
    public class SingleValueTypeConfiguration : IEntityTypeConfiguration<SingleValueType>
    {
        public void Configure(EntityTypeBuilder<SingleValueType> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}
