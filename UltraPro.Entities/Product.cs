using System;
using System.Collections.Generic;
using System.Text;
using UltraPro.Entities.Core;

namespace UltraPro.Entities
{
    public class Product : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
