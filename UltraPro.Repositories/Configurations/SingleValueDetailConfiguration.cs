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
    public class SingleValueDetailConfiguration : IEntityTypeConfiguration<SingleValueDetail>
    {
        public void Configure(EntityTypeBuilder<SingleValueDetail> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.Description)
                .IsRequired(false)
                .HasMaxLength(100);

            builder.Property(x => x.Sequence);

            builder.Property(x => x.TypeId)
                .IsRequired();

            builder.HasOne(x => x.Type)
                .WithMany(x => x.SingleValueDetails)
                .HasForeignKey(x => x.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
