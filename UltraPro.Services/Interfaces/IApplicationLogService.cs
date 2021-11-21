using System;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Common.Model;

namespace UltraPro.Services.Interfaces
{
    public interface IApplicationLogService : IDisposable
    {
        Task<QueryResult<ApplicationLog>> GetAllAsync(ApplicationLogQuery queryObj);
        Task<ApplicationLog> GetByIdAsync(long id);
    }
}
