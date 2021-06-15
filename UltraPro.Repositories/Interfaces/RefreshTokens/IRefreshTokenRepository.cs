using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;

namespace UltraPro.Repositories.Interfaces.RefreshTokens
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken, Guid, ApplicationDbContext>
    {
        
    }
}
