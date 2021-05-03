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
using UltraPro.Repositories.Interfaces.RequestResponseLogs;
using UltraPro.Repositories.Interfaces.SingleValue;
using UltraPro.Services.Exceptions;
using UltraPro.Services.Interfaces;

namespace UltraPro.Services.Implements
{
    public class RequestResponseLogService : IRequestResponseLogService
    {        
        private readonly IRequestResponseLogUnitOfWork _requestResponseLogUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;


        public RequestResponseLogService(
            IRequestResponseLogUnitOfWork requestResponseLogUnitOfWork,
            ICurrentUserService currentUserService,
            IDateTime dateTime
            )
        {
            _requestResponseLogUnitOfWork = requestResponseLogUnitOfWork;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<QueryResult<RequestResponseLog>> GetAllAsync(RequestResponseLogQuery queryObj)
        {
            var queryResult = new QueryResult<RequestResponseLog>();

            var columnsMap = new Dictionary<string, Expression<Func<RequestResponseLog, object>>>()
            {
                ["userId"] = v => v.UserId,
            };

            var result = await _requestResponseLogUnitOfWork.RequestResponseLogRepository.GetAsync(x => x,
                            x => (string.IsNullOrEmpty(queryObj.GlobalSearchValue) || x.UserId.ToString().ToLower() == queryObj.GlobalSearchValue.ToLower()),
                            x => x.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending),
                            null, queryObj.Page, queryObj.PageSize);           

            queryResult.Items = result.Items;
            queryResult.Total = result.Total;
            queryResult.TotalFilter = result.TotalFilter;

            return queryResult;
        }

        public async Task<RequestResponseLog> GetByIdAsync(Guid id)
        {
            var singleValue = await _requestResponseLogUnitOfWork.RequestResponseLogRepository.GetByIdAsync(id);

            _ = singleValue ?? throw new NotFoundException(nameof(RequestResponseLog), id);

            return singleValue;
        }

        public async Task<Guid> AddAsync(RequestResponseLog entity)
        {
            await _requestResponseLogUnitOfWork.RequestResponseLogRepository.AddAsync(entity);
            await _requestResponseLogUnitOfWork.SaveChangesAsync();

            return entity.Id;
        }

        public void Dispose()
        {
            _requestResponseLogUnitOfWork.Dispose();
        }
    }
}
