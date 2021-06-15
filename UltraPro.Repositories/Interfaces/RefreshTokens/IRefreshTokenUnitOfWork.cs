using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Repositories.Core;

namespace UltraPro.Repositories.Interfaces.RefreshTokens
{
    public interface IRefreshTokenUnitOfWork : IUnitOfWork
    {
        public IRefreshTokenRepository RefreshTokenRepository { get; set; }
    }
}
