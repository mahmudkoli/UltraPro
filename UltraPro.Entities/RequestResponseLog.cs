using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Entities
{
    public class RequestResponseLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public string Path { get; set; }
        public string QueryString { get; set; }
        public string Method { get; set; }
        public string ContentType { get; set; }
        public string Payload { get; set; }
        public string Response { get; set; }
        public int ResponseCode { get; set; }
        public DateTime RequestedOn { get; set; }
        public DateTime RespondedOn { get; set; }
        public bool IsSuccessStatusCode { get; set; }
    }
}
