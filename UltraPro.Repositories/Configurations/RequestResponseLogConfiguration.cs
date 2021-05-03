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
    public class RequestResponseLogConfiguration : IEntityTypeConfiguration<RequestResponseLog>
    {
        public void Configure(EntityTypeBuilder<RequestResponseLog> builder)
        {

        }
    }
}
