using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.AuditLogs;

namespace UltraPro.Repositories.Implements.AuditLogs
{
    public class AuditLogRepository : Repository<AuditLog, Guid, ApplicationDbContext>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
