using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Services.Exceptions
{
    public class ValidationError
    {
        public string PropertyName { get; set; }
        public IList<string> PropertyFailures { get; set; }
    }
}
