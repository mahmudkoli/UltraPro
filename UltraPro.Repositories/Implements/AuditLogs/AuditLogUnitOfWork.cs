using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.AuditLogs;

namespace UltraPro.Repositories.Implements.AuditLogs
{
    public class AuditLogUnitOfWork : UnitOfWork, IAuditLogUnitOfWork
    {
        public IAuditLogRepository AuditLogRepository { get; set; }

        public AuditLogUnitOfWork(
            ApplicationDbContext dbContext, 
            IAuditLogRepository requestResponseLogRepository
            ) : base(dbContext)
        {
            this.AuditLogRepository = requestResponseLogRepository;
        }
    }
}
