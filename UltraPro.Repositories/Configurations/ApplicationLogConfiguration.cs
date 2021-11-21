using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UltraPro.Entities;

namespace UltraPro.Repositories.Configurations
{
    public class ApplicationLogConfiguration : IEntityTypeConfiguration<ApplicationLog>
    {
        public void Configure(EntityTypeBuilder<ApplicationLog> builder)
        {
            builder.Property(x => x.Id)
                //.HasDefaultValueSql("NEWID()");
                //.HasDefaultValueSql("NEWSEQUENTIALID()");
                //.HasColumnType("uniqueidentifier");
                .HasColumnType("bigint");

            builder.Property(x => x.Level)
                .HasMaxLength(128);

            builder.Property(x => x.TimeStamp)
                .HasColumnType("datetimeoffset(7)");

            builder.Property(x => x.Properties)
                .HasColumnType("xml");

            builder.Property(x => x.UserName)
                .HasMaxLength(200);

            builder.Property(x => x.IP)
                .HasColumnType("varchar")
                .HasMaxLength(200);
        }
    }
}
