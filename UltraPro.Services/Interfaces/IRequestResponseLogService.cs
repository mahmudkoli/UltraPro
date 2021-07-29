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
    public interface IRequestResponseLogService : IDisposable
    {
        Task<QueryResult<RequestResponseLog>> GetAllAsync(RequestResponseLogQuery queryObj);
        Task<RequestResponseLog> GetByIdAsync(Guid id);
        Task<Guid> AddAsync(RequestResponseLog entity);
    }
}
