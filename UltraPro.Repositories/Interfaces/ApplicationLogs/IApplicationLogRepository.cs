using System;
using UltraPro.Entities;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;

namespace UltraPro.Repositories.Interfaces.ApplicationLogs
{
    public interface IApplicationLogRepository : IRepository<ApplicationLog, long, ApplicationDbContext>
    {
        
    }
}
