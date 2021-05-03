using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.RequestResponseLogs;

namespace UltraPro.Repositories.Implements.RequestResponseLogs
{
    public class RequestResponseLogUnitOfWork : UnitOfWork, IRequestResponseLogUnitOfWork
    {
        public IRequestResponseLogRepository RequestResponseLogRepository { get; set; }

        public RequestResponseLogUnitOfWork(
            ApplicationDbContext dbContext, 
            IRequestResponseLogRepository requestResponseLogRepository
            ) : base(dbContext)
        {
            this.RequestResponseLogRepository = requestResponseLogRepository;
        }
    }
}
