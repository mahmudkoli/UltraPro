using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Entities.Core
{
    public interface IEntity
    {
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
