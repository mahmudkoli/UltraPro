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
using UltraPro.Repositories.Interfaces.SingleValue;
using UltraPro.Services.Exceptions;
using UltraPro.Services.Interfaces;

namespace UltraPro.Services.Implements
{
    public class AuditLogService : IAuditLogService
    {        
        private readonly IAuditLogUnitOfWork _auditLogUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;


        public AuditLogService(
            IAuditLogUnitOfWork auditLogUnitOfWork,
            ICurrentUserService currentUserService,
            IDateTime dateTime
            )
        {
            _auditLogUnitOfWork = auditLogUnitOfWork;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<QueryResult<AuditLog>> GetAllAsync(AuditLogQuery queryObj)
        {
            var queryResult = new QueryResult<AuditLog>();

            var columnsMap = new Dictionary<string, Expression<Func<AuditLog, object>>>()
            {
                ["userId"] = v => v.UserId,
            };

            var result = await _auditLogUnitOfWork.AuditLogRepository.GetAsync(x => x,
                            x => (string.IsNullOrEmpty(queryObj.GlobalSearchValue) || x.UserId.ToString().ToLower() == queryObj.GlobalSearchValue.ToLower()),
                            x => x.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending),
                            null, queryObj.Page, queryObj.PageSize);           

            queryResult.Items = result.Items;
            queryResult.Total = result.Total;
            queryResult.TotalFilter = result.TotalFilter;

            return queryResult;
        }

        public async Task<AuditLog> GetByIdAsync(Guid id)
        {
            var singleValue = await _auditLogUnitOfWork.AuditLogRepository.GetByIdAsync(id);

            _ = singleValue ?? throw new NotFoundException(nameof(AuditLog), id);

            return singleValue;
        }

        public void Dispose()
        {
            _auditLogUnitOfWork.Dispose();
        }
    }
}
