using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Model;

namespace UltraPro.Entities.NotMapped
{
    public class SingleValueQuery : IQueryObject
    {
        public string TypeName { get; set; }
        public string TypeCode { get; set; }
        public string DetailName { get; set; }
    }
}


