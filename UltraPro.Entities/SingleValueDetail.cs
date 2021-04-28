using System;
using UltraPro.Entities.Core;

namespace UltraPro.Entities
{
    public class SingleValueDetail : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }
        public int TypeId { get; set; }
        public SingleValueType Type { get; set; }
    }
}
