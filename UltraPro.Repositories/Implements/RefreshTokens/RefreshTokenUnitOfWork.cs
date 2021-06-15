using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Context;
using UltraPro.Repositories.Core;
using UltraPro.Repositories.Interfaces.RefreshTokens;

namespace UltraPro.Repositories.Implements.RefreshTokens
{
    public class RefreshTokenUnitOfWork : UnitOfWork, IRefreshTokenUnitOfWork
    {
        public IRefreshTokenRepository RefreshTokenRepository { get; set; }

        public RefreshTokenUnitOfWork(
            ApplicationDbContext dbContext,
            IRefreshTokenRepository refreshTokenRepository
            ) : base(dbContext)
        {
            this.RefreshTokenRepository = refreshTokenRepository;
        }
    }
}
