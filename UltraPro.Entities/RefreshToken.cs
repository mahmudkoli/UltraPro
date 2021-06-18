using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities.Core;

namespace UltraPro.Entities
{
    public class RefreshToken : IKey<Guid>
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public bool IsActive => Revoked == null && !IsExpired;

        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
