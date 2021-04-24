using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.API.Core
{
    public class ValidationError
    {
        public string PropertyName { get; set; }
        public IList<string> Errors { get; set; }
    }
}
