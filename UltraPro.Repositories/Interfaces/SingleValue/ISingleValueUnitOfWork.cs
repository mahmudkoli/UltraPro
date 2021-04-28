using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Core;

namespace UltraPro.Repositories.Interfaces.SingleValue
{
    public interface ISingleValueUnitOfWork : IUnitOfWork
    {
        public ISingleValueTypeRepository SingleValueTypeRepository { get; set; }
        public ISingleValueDetailRepository SingleValueDetailRepository { get; set; }
    }
}
