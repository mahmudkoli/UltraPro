using System;
using System.Collections.Generic;
using UltraPro.Entities.Core;

namespace UltraPro.Entities
{
    public class SingleValueType : AuditableEntity<int>
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public IList<SingleValueDetail> SingleValueDetails { get; set; }

        public SingleValueType()
        {
            this.SingleValueDetails = new List<SingleValueDetail>();
        }
    }
}
