using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.RequestResponseLogs;

namespace UltraPro.Repositories.Implements.RequestResponseLogs
{
    public class RequestResponseLogRepository : Repository<RequestResponseLog, int, ApplicationDbContext>, IRequestResponseLogRepository
    {
        public RequestResponseLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
