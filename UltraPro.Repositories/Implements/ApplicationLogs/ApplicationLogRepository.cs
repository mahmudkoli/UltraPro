using System;
using UltraPro.Entities;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.ApplicationLogs;

namespace UltraPro.Repositories.Implements.ApplicationLogs
{
    public class ApplicationLogRepository : Repository<ApplicationLog, long, ApplicationDbContext>, IApplicationLogRepository
    {
        public ApplicationLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
