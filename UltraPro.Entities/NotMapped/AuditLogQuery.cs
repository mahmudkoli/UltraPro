using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Entities.NotMapped
{
    public class AuditLogQuery : IQueryObject
    {
        public Guid UserId { get; set; }
    }
}


