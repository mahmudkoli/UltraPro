using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.SingleValue;

namespace UltraPro.Repositories.Implements.SingleValue
{
    public class SingleValueDetailRepository : Repository<SingleValueDetail, int, ApplicationDbContext>, ISingleValueDetailRepository
    {
        public SingleValueDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
