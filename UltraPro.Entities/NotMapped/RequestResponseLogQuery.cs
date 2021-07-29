using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Model;

namespace UltraPro.Entities.NotMapped
{
    public class RequestResponseLogQuery : IQueryObject
    {
        public Guid UserId { get; set; }
    }
}


