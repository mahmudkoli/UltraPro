using System;
using UltraPro.Entities.Core;

namespace UltraPro.Entities
{
    public class ApplicationLog : IKey<long>
    {
        public long Id { get; set; }
        public string Message { get; set; }
        public string MessageTemplate { get; set; }
        public string Level { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
        public string LogEvent { get; set; }
        public string UserName { get; set; }
        public string IP { get; set; }
    }
}
