using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.RefreshTokens;

namespace UltraPro.Repositories.Implements.RefreshTokens
{
    public class RefreshTokenRepository : Repository<Entities.RefreshToken, Guid, ApplicationDbContext>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
