using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.API.Mappings;
using UltraPro.Entities;

namespace UltraPro.API.Models.Logs
{
    public class RequestResponseLogModel : IMapFrom<RequestResponseLog>
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

        public RequestResponseLogModel()
        {

        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<RequestResponseLog, RequestResponseLogModel>();
        }
    }
}
