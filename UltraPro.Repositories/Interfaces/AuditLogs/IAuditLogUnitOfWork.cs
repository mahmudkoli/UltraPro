using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Core;

namespace UltraPro.Repositories.Interfaces.AuditLogs
{
    public interface IAuditLogUnitOfWork : IUnitOfWork
    {
        public IAuditLogRepository AuditLogRepository { get; set; }
    }
}
