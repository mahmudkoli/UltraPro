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
using UltraPro.Repositories.Interfaces.SingleValue;
using UltraPro.Common.Exceptions;
using UltraPro.Services.Interfaces;
using UltraPro.Common.Model;

namespace UltraPro.Services.Implements
{
    public class SingleValueService : ISingleValueService
    {        
        private readonly ISingleValueUnitOfWork _singleValueUnitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;


        public SingleValueService(
            ISingleValueUnitOfWork singleValueUnitOfWork,
            ICurrentUserService currentUserService,
            IDateTime dateTime
            )
        {
            _singleValueUnitOfWork = singleValueUnitOfWork;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        #region Single Value Detail
        public async Task<QueryResult<SingleValueDetail>> GetAllAsync(SingleValueQuery queryObj)
        {
            var queryResult = new QueryResult<SingleValueDetail>();

            var columnsMap = new Dictionary<string, Expression<Func<SingleValueDetail, object>>>()
            {
                ["name"] = v => v.Name,
                ["typeName"] = v => v.Type.Name,
                ["typeCode"] = v => v.Type.Code,
            };

            var result = await _singleValueUnitOfWork.SingleValueDetailRepository.GetAsync(x => x,
                            x => (string.IsNullOrEmpty(queryObj.GlobalSearchValue) || x.Name.Contains(queryObj.GlobalSearchValue) || 
                                    x.Type.Name.Contains(queryObj.GlobalSearchValue) || x.Type.Code.Contains(queryObj.GlobalSearchValue)),
                            x => x.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending),
                            null, queryObj.Page, queryObj.PageSize);           

            queryResult.Items = result.Items;
            queryResult.Total = result.Total;
            queryResult.TotalFilter = result.TotalFilter;

            return queryResult;
        }

        public async Task<SingleValueDetail> GetByIdAsync(int id)
        {
            var singleValue = await _singleValueUnitOfWork.SingleValueDetailRepository.GetByIdAsync(id);

            _ = singleValue ?? throw new NotFoundException(nameof(SingleValueDetail), id);

            return singleValue;
        }

        public async Task<int> AddAsync(SingleValueDetail entity)
        {
            var isExists = await this.IsExistsAsync(entity.Name, entity.TypeId, entity.Id);
            if (isExists) throw new DuplicationException(nameof(entity.Name));

            await _singleValueUnitOfWork.SingleValueDetailRepository.AddAsync(entity);
            await _singleValueUnitOfWork.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<int> UpdateAsync(SingleValueDetail entity)
        {
            var isExists = await this.IsExistsAsync(entity.Name, entity.TypeId, entity.Id);
            if (isExists) throw new DuplicationException(nameof(entity.Name));

            var existingEntity = await _singleValueUnitOfWork.SingleValueDetailRepository.GetByIdAsync(entity.Id);
            _ = existingEntity ?? throw new NotFoundException(nameof(SingleValueDetail), entity.Id);

            existingEntity.Name = entity.Name;
            existingEntity.Description = entity.Description;
            existingEntity.Sequence = entity.Sequence;
            existingEntity.TypeId = entity.TypeId;

            await _singleValueUnitOfWork.SingleValueDetailRepository.UpdateAsync(existingEntity);
            await _singleValueUnitOfWork.SaveChangesAsync();

            return entity.Id;
        }

        public Task<bool> ActiveInactiveAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _singleValueUnitOfWork.SingleValueDetailRepository.DeleteAsync(id);
            return await _singleValueUnitOfWork.SaveChangesAsync();
        }

        public async Task<IList<SingleValueDetail>> GetAllActiveAsync(string typeCode)
        {
            return (await _singleValueUnitOfWork.SingleValueDetailRepository.GetAsync(x => x,
                            x => x.IsActive && !x.IsDeleted && x.Type.Code.ToLower() == typeCode.ToLower(), 
                            x => x.OrderBy(o => o.Name), 
                            null, true));
        }

        public async Task<IList<SingleValueDetail>> GetAllActiveAsync(int typeId)
        {
            return (await _singleValueUnitOfWork.SingleValueDetailRepository.GetAsync(x => x,
                            x => x.IsActive && !x.IsDeleted && x.TypeId == typeId, 
                            x => x.OrderBy(o => o.Name), 
                            null, true));
        }

        public async Task<IList<KeyValuePairObject>> GetAllForSelectAsync(int typeId)
        {
            return (await _singleValueUnitOfWork.SingleValueDetailRepository.GetAsync(
                            x => new KeyValuePairObject { Value = x.Id, Text = x.Name },
                            x => x.IsActive && !x.IsDeleted && x.TypeId == typeId,
                            x => x.OrderBy(o => o.Name),
                            null, true));
        }

        public async Task<IList<KeyValuePairObject>> GetAllForSelectAsync(string typeCode)
        {
            return (await _singleValueUnitOfWork.SingleValueDetailRepository.GetAsync(
                            x => new KeyValuePairObject { Value = x.Id, Text = x.Name },
                            x => x.IsActive && !x.IsDeleted && x.Type.Code.ToLower() == typeCode.ToLower(),
                            x => x.OrderBy(o => o.Name),
                            null, true));
        }

        public async Task<bool> IsExistsAsync(string name, int typeId, int id)
        {
            var result = await _singleValueUnitOfWork.SingleValueDetailRepository.IsExistsAsync(x =>
                            (x.Name.ToLower() == name.ToLower()) && x.TypeId == typeId && x.Id != id && !x.IsDeleted);
            return result;
        }
        #endregion

        #region Single Value Type
        public async Task<IList<SingleValueType>> GetAllActiveTypeAsync()
        {
            return (await _singleValueUnitOfWork.SingleValueTypeRepository.GetAsync(x => x,
                            x => x.IsActive && !x.IsDeleted,
                            x => x.OrderBy(o => o.Name),
                            null, true));
        }

        public async Task<IList<KeyValuePairObject>> GetAllTypeForSelectAsync()
        {
            return (await _singleValueUnitOfWork.SingleValueTypeRepository.GetAsync(
                            x => new KeyValuePairObject { Value = x.Id, Text = x.Name },
                            x => x.IsActive && !x.IsDeleted,
                            x => x.OrderBy(o => o.Name),
                            null, true));
        }
        #endregion

        public void Dispose()
        {
            _singleValueUnitOfWork.Dispose();
        }
    }
}
