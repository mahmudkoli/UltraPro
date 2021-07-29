using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Common.Model;

namespace UltraPro.Services.Interfaces
{
    public interface IAuditLogService : IDisposable
    {
        Task<QueryResult<AuditLog>> GetAllAsync(AuditLogQuery queryObj);
        Task<AuditLog> GetByIdAsync(Guid id);
    }
}
