using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using UltraPro.Common.Services;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Repositories.Extensions;
using UltraPro.Repositories.Interfaces.AuditLogs;
using UltraPro.Repositories.Interfaces.RefreshTokens;
using UltraPro.Repositories.Interfaces.SingleValue;
using UltraPro.Common.Exceptions;
using UltraPro.Services.Interfaces;

namespace UltraPro.Services.Implements
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenUnitOfWork _refreshTokenUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;


        public RefreshTokenService(
            IRefreshTokenUnitOfWork refreshTokenUnitOfWork,
            ICurrentUserService currentUserService,
            IDateTime dateTime
            )
        {
            _refreshTokenUnitOfWork = refreshTokenUnitOfWork;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<RefreshToken> GetByIdAsync(Guid id)
        {
            var singleValue = await _refreshTokenUnitOfWork.RefreshTokenRepository.GetByIdAsync(id);

            _ = singleValue ?? throw new NotFoundException(nameof(RefreshToken), id);

            return singleValue;
        }

        public async Task<RefreshToken> GetByTokenAsync(string token)
        {
            var singleValue = await _refreshTokenUnitOfWork.RefreshTokenRepository.GetFirstOrDefaultAsync(x => x, 
                                        x => x.Token == token,
                                        null, null, true);

            _ = singleValue ?? throw new NotFoundException(nameof(RefreshToken), token);

            return singleValue;
        }

        public async Task<Guid> AddAsync(RefreshToken entity)
        {
            await _refreshTokenUnitOfWork.RefreshTokenRepository.AddAsync(entity);
            await _refreshTokenUnitOfWork.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Guid> UpdateAsync(RefreshToken entity)
        {
            await _refreshTokenUnitOfWork.RefreshTokenRepository.UpdateAsync(entity);
            await _refreshTokenUnitOfWork.SaveChangesAsync();

            return entity.Id;
        }

        public void Dispose()
        {
            _refreshTokenUnitOfWork.Dispose();
        }
    }
}
