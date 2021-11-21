using UltraPro.Repositories.Core;

namespace UltraPro.Repositories.Interfaces.ApplicationLogs
{
    public interface IApplicationLogUnitOfWork : IUnitOfWork
    {
        public IApplicationLogRepository ApplicationLogRepository { get; set; }
    }
}
