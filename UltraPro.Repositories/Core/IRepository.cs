using UltraPro.Entities.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Repositories.Core
{
    public interface IRepository<TEntity, TKey, TContext>
        where TEntity : class, IKey<TKey>, new()
        where TContext : DbContext
    {
        #region LINQ Async
        Task<IList<TResult>> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false);
        Task<(IList<TResult> Items, int Total, int TotalFilter)> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            int pageIndex = 1, int pageSize = 10,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false);
        Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false);
        Task<TEntity> GetByIdAsync(TKey id, bool ignoreQueryFilters = false);
        Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false);
        Task<long> GetSumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false);
        Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters = false);
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IList<TEntity> entities);
        Task UpdateAsync(TEntity entity);
        Task UpdateRangeAsync(IList<TEntity> entities);
        Task DeleteAsync(object id, bool hardDelete = false);
        Task DeleteAsync(TEntity entity, bool hardDelete = false);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool hardDelete = false);
        Task DeleteRangeAsync(IList<TEntity> entities, bool hardDelete = false);
        #endregion

        #region LINQ
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true, bool ignoreQueryFilters = false);
        IList<TResult> Get<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false);
        (IList<TResult> Items, int Total, int TotalFilter) Get<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            int pageIndex = 1, int pageSize = 10,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false);
        TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false);
        TEntity GetById(TKey id, bool ignoreQueryFilters = false);
        int GetCount(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false);
        long GetSum(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false);
        bool IsExists(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters = false);
        void Add(TEntity entity);
        void AddRange(IList<TEntity> entities);
        void Update(TEntity entity);
        void UpdateRange(IList<TEntity> entities);
        void Delete(object id, bool hardDelete = false);
        void Delete(TEntity entity, bool hardDelete = false);
        void Delete(Expression<Func<TEntity, bool>> predicate, bool hardDelete = false);
        void DeleteRange(IList<TEntity> entities, bool hardDelete = false);
        #endregion

        #region SQL
        IList<TEntity> ExecuteSqlQuery(string sql, params object[] parameters);
        int ExecuteSqlCommand(string sql, params object[] parameters);
        IList<dynamic> GetFromSql(string sql, Dictionary<string, object> parameters, bool isStoredProcedure = false);
        DataSet GetDataSetFromSql(string sql, Dictionary<string, object> parameters, bool isStoredProcedure = false);
        (IList<TEntity> Items, int Total, int TotalFilter) GetFromSql(string sql, IList<(string Key, object Value, bool IsOut)> parameters, bool isStoredProcedure = true);
        (IList<dynamic> Items, int Total, int TotalFilter) GetFromSql(string sql, IList<(string Key, object Value, bool IsOut)> parameters);
        #endregion
    }
}
