using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;

namespace UltraPro.Services.Interfaces
{
    public interface IRefreshTokenService : IDisposable
    {
        Task<RefreshToken> GetByIdAsync(Guid id);
        Task<RefreshToken> GetByTokenAsync(string token);
        Task<Guid> AddAsync(RefreshToken entity);
        Task<Guid> UpdateAsync(RefreshToken entity);
    }
}
