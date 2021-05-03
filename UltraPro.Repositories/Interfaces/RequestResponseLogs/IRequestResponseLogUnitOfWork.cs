using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Core;

namespace UltraPro.Repositories.Interfaces.RequestResponseLogs
{
    public interface IRequestResponseLogUnitOfWork : IUnitOfWork
    {
        public IRequestResponseLogRepository RequestResponseLogRepository { get; set; }
    }
}
