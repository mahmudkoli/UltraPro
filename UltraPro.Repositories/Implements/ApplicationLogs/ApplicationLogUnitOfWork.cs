using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.ApplicationLogs;

namespace UltraPro.Repositories.Implements.ApplicationLogs
{
    public class ApplicationLogUnitOfWork : UnitOfWork, IApplicationLogUnitOfWork
    {
        public IApplicationLogRepository ApplicationLogRepository { get; set; }

        public ApplicationLogUnitOfWork(
            ApplicationDbContext dbContext, 
            IApplicationLogRepository requestResponseLogRepository
            ) : base(dbContext)
        {
            this.ApplicationLogRepository = requestResponseLogRepository;
        }
    }
}
