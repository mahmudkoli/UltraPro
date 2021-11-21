using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UltraPro.Common.Services;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Repositories.Extensions;
using UltraPro.Repositories.Interfaces.ApplicationLogs;
using UltraPro.Common.Exceptions;
using UltraPro.Services.Interfaces;
using UltraPro.Common.Model;

namespace UltraPro.Services.Implements
{
    public class ApplicationLogService : IApplicationLogService
    {        
        private readonly IApplicationLogUnitOfWork _auditLogUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;


        public ApplicationLogService(
            IApplicationLogUnitOfWork auditLogUnitOfWork,
            ICurrentUserService currentUserService,
            IDateTime dateTime
            )
        {
            _auditLogUnitOfWork = auditLogUnitOfWork;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<QueryResult<ApplicationLog>> GetAllAsync(ApplicationLogQuery queryObj)
        {
            var queryResult = new QueryResult<ApplicationLog>();

            var columnsMap = new Dictionary<string, Expression<Func<ApplicationLog, object>>>()
            {
                ["level"] = v => v.Level,
            };

            var result = await _auditLogUnitOfWork.ApplicationLogRepository.GetAsync(x => x,
                            x => (string.IsNullOrEmpty(queryObj.GlobalSearchValue) || x.Level.ToLower() == queryObj.GlobalSearchValue.ToLower()),
                            x => x.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending),
                            null, queryObj.Page, queryObj.PageSize);           

            queryResult.Items = result.Items;
            queryResult.Total = result.Total;
            queryResult.TotalFilter = result.TotalFilter;

            return queryResult;
        }

        public async Task<ApplicationLog> GetByIdAsync(long id)
        {
            var singleValue = await _auditLogUnitOfWork.ApplicationLogRepository.GetByIdAsync(id);

            _ = singleValue ?? throw new NotFoundException(nameof(ApplicationLog), id);

            return singleValue;
        }

        public void Dispose()
        {
            _auditLogUnitOfWork.Dispose();
        }
    }
}
