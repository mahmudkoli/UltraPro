using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Mappings;
using UltraPro.Common.Enums;
using UltraPro.Entities;

namespace UltraPro.API.Models.Logs
{
    public class AuditLogModel : IMapFrom<AuditLog>
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public EnumAuditType Type { get; set; }
        public string TableName { get; set; }
        public DateTime DateTime { get; set; }
        public string KeyValues { get; set; }
        public string OldValues { get; set; }
        public string NewValues { get; set; }
        public string ChangedColumns { get; set; }

        public AuditLogModel()
        {

        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AuditLog, AuditLogModel>();
        }
    }
}
