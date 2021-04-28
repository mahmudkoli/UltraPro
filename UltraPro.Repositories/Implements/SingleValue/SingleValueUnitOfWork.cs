using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.SingleValue;

namespace UltraPro.Repositories.Implements.SingleValue
{
    public class SingleValueUnitOfWork : UnitOfWork, ISingleValueUnitOfWork
    {
        public ISingleValueTypeRepository SingleValueTypeRepository { get; set; }
        public ISingleValueDetailRepository SingleValueDetailRepository { get; set; }

        public SingleValueUnitOfWork(
            ApplicationDbContext dbContext, 
            ISingleValueTypeRepository singleValueTypeRepository,
            ISingleValueDetailRepository singleValueDetailRepository
            ) : base(dbContext)
        {
            this.SingleValueTypeRepository = singleValueTypeRepository;
            this.SingleValueDetailRepository = singleValueDetailRepository;
        }
    }
}
