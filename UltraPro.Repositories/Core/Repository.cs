using UltraPro.Entities.Core;
using UltraPro.Repositories.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UltraPro.Repositories.Core
{
    public abstract class Repository<TEntity, TKey, TContext>
        : IRepository<TEntity, TKey, TContext>
        where TEntity : class, IKey<TKey>, new()
        where TContext : DbContext
    {
        #region CONFIG
        protected TContext _dbContext;
        protected DbSet<TEntity> _dbSet;

        public Repository(TContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }
        #endregion

        #region LINQ Async
        public virtual async Task<IList<TResult>> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (disableTracking)
                query = query.AsNoTracking();

            var result = await query.Select(selector).ToListAsync();

            return result;
        }

        public virtual async Task<(IList<TResult> Items, int Total, int TotalFilter)> GetAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            int pageIndex = 1, int pageSize = 10,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            int total = await query.CountAsync();
            int totalFilter = total;

            if (include != null)
                query = include(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
                totalFilter = await query.CountAsync();
            }

            if (orderBy != null)
                query = orderBy(query);

            if (disableTracking)
                query = query.AsNoTracking();

            var result = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(selector).ToListAsync();

            return (result, total, totalFilter);
        }

        public virtual async Task<TResult> GetFirstOrDefaultAsync<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = true)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (disableTracking)
                query = query.AsNoTracking();

            var result = await query.Select(selector).FirstOrDefaultAsync();

            return result;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            var result = await query.FirstOrDefaultAsync(x => x.Id.Equals(id));

            return result;
        }

        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync();
        }

        public virtual async Task<long> GetSumAsync(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.SumAsync(selector);
        }

        public virtual async Task<bool> IsExistsAsync(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            return await query.AnyAsync(predicate);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            var entry = _dbContext.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Added;
        }

        public virtual async Task AddRangeAsync(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.Entry(entity);
                _dbSet.Attach(entity);
                entry.State = EntityState.Added;
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
                _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public virtual async Task UpdateRangeAsync(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                    _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }

        public virtual async Task DeleteAsync(object id, bool hardDelete = false)
        {
            var entity = await _dbSet.FindAsync(id);

            await DeleteAsync(entity, hardDelete);
        }

        public virtual async Task DeleteAsync(TEntity entity, bool hardDelete = false)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
                _dbSet.Attach(entity);

            if (hardDelete)
                entry.State = EntityState.Deleted;
            else
            {
                //entity.GetType().GetProperty(nameof(IEntity.IsDeleted)).SetValue(entity, true);
                //entry.Property(nameof(IEntity.IsDeleted)).CurrentValue = true;
                if (entry.Entity is IEntity en)
                    en.IsDeleted = true;
                entry.State = EntityState.Modified;
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, bool hardDelete = false)
        {
            var query = _dbSet.AsQueryable().Where(predicate);

            foreach (var entity in query)
            {
                var entry = _dbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                    _dbSet.Attach(entity);

                if (hardDelete)
                    entry.State = EntityState.Deleted;
                else
                {
                    if (entry.Entity is IEntity en)
                        en.IsDeleted = true;
                    entry.State = EntityState.Modified;
                }
            }
        }

        public virtual async Task DeleteRangeAsync(IList<TEntity> entities, bool hardDelete = false)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                    _dbSet.Attach(entity);

                if (hardDelete)
                    entry.State = EntityState.Deleted;
                else
                {
                    if (entry.Entity is IEntity en)
                        en.IsDeleted = true;
                    entry.State = EntityState.Modified;
                }
            }
        }
        #endregion

        #region LINQ
        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null, bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (predicate != null)
                query = query.Where(predicate);

            if (disableTracking)
                query = query.AsNoTracking();

            return query;
        }

        public virtual IList<TResult> Get<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (disableTracking)
                query = query.AsNoTracking();

            var result = query.Select(selector).ToList();

            return result;
        }

        public virtual (IList<TResult> Items, int Total, int TotalFilter) Get<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            int pageIndex = 1, int pageSize = 10,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            int total = query.Count();
            int totalFilter = total;

            if (include != null)
                query = include(query);

            if (predicate != null)
            {
                query = query.Where(predicate);
                totalFilter = query.Count();
            }

            if (orderBy != null)
                query = orderBy(query);

            if (disableTracking)
                query = query.AsNoTracking();

            var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(selector).ToList();

            return (result, total, totalFilter);
        }

        public virtual TResult GetFirstOrDefault<TResult>(Expression<Func<TEntity, TResult>> selector,
                            Expression<Func<TEntity, bool>> predicate = null,
                            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
                            bool disableTracking = true,
                            bool ignoreQueryFilters = true)
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (disableTracking)
                query = query.AsNoTracking();

            var result = query.Select(selector).FirstOrDefault();

            return result;
        }

        public virtual TEntity GetById(TKey id, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            var result = query.FirstOrDefault(x => x.Id.Equals(id));

            return result;
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query.Count();
        }

        public virtual long GetSum(Expression<Func<TEntity, long>> selector, Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return query.Sum(selector);
        }

        public virtual bool IsExists(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters = false)
        {
            var query = _dbSet.AsQueryable();

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            return query.Any(predicate);
        }

        public virtual void Add(TEntity entity)
        {
            var entry = _dbContext.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Added;
        }

        public virtual void AddRange(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.Entry(entity);
                _dbSet.Attach(entity);
                entry.State = EntityState.Added;
            }
        }

        public virtual void Update(TEntity entity)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
                _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public virtual void UpdateRange(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                var entry = _dbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                    _dbSet.Attach(entity);
                entry.State = EntityState.Modified;
            }
        }

        public virtual void Delete(object id, bool hardDelete = false)
        {
            var entity = _dbSet.Find(id);

            Delete(entity, hardDelete);
        }

        public virtual void Delete(TEntity entity, bool hardDelete = false)
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
                _dbSet.Attach(entity);

            if (hardDelete)
                entry.State = EntityState.Deleted;
            else
            {
                //entity.GetType().GetProperty(nameof(IEntity.IsDeleted)).SetValue(entity, true);
                //entry.Property(nameof(IEntity.IsDeleted)).CurrentValue = true;
                if (entry.Entity is IEntity en)
                    en.IsDeleted = true;
                entry.State = EntityState.Modified;
            }
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate, bool hardDelete = false)
        {
            var query = _dbSet.AsQueryable().Where(predicate);

            if (query.Any())
            {
                foreach (var entity in query)
                {
                    var entry = _dbContext.Entry(entity);
                    if (entry.State == EntityState.Detached)
                        _dbSet.Attach(entity);

                    if (hardDelete)
                        entry.State = EntityState.Deleted;
                    else
                    {
                        if (entry.Entity is IEntity en)
                            en.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }
                }
            }
        }

        public virtual void DeleteRange(IList<TEntity> entities, bool hardDelete = false)
        {

            if (entities.Any())
            {
                foreach (var entity in entities)
                {
                    var entry = _dbContext.Entry(entity);
                    if (entry.State == EntityState.Detached)
                        _dbSet.Attach(entity);

                    if (hardDelete)
                        entry.State = EntityState.Deleted;
                    else
                    {
                        if (entry.Entity is IEntity en)
                            en.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }
                }
            }
        }
        #endregion

        #region SQL
        public IList<TEntity> ExecuteSqlQuery(string sql, params object[] parameters)
        {
            return (IList<TEntity>)_dbSet.FromSqlRaw(sql, parameters).ToList();
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlCommand(sql, parameters);
        }

        public IList<dynamic> GetFromSql(string sql, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            var items = new List<dynamic>();

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                if (isStoredProcedure) { command.CommandType = CommandType.StoredProcedure; }
                if (command.Connection.State != ConnectionState.Open) { command.Connection.Open(); }

                foreach (var param in parameters)
                {
                    DbParameter dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value;
                    command.Parameters.Add(dbParameter);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ExpandoObject() as IDictionary<string, object>;
                        for (var count = 0; count < reader.FieldCount; count++)
                        {
                            item.Add(reader.GetName(count), reader[count]);
                        }
                        items.Add(item);
                    }
                }
            }

            return items;
        }
        
        public DataSet GetDataSetFromSql(string sql, Dictionary<string, object> parameters, bool isStoredProcedure = false)
        {
            var dataSet = new DataSet();
            DbConnection connection = _dbContext.Database.GetDbConnection();
            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(connection);

            using (var command = dbFactory.CreateCommand())
            {
                command.Connection = connection;
                command.CommandText = sql;
                if (isStoredProcedure) { command.CommandType = CommandType.StoredProcedure; }
                if (command.Connection.State != ConnectionState.Open) { command.Connection.Open(); }

                foreach (var param in parameters)
                {
                    DbParameter dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    dbParameter.Value = param.Value;
                    command.Parameters.Add(dbParameter);
                }

                using (var adapter = dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = command;
                    adapter.Fill(dataSet);
                }
            }

            return dataSet;
        }

        public (IList<TEntity> Items, int Total, int TotalFilter) GetFromSql(string sql, IList<(string Key, object Value, bool IsOut)> parameters, bool isStoredProcedure = true)
        {
            var items = new List<TEntity>();
            int? totalCount = 0;
            int? filteredCount = 0;

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                if (isStoredProcedure) { command.CommandType = CommandType.StoredProcedure; }
                if (command.Connection.State != ConnectionState.Open) { command.Connection.Open(); }

                foreach (var param in parameters)
                {
                    DbParameter dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    if (!param.IsOut)
                    {
                        dbParameter.Value = param.Value;
                    }
                    else
                    {
                        dbParameter.Direction = ParameterDirection.Output;
                        dbParameter.DbType = DbType.Int32;
                    }
                    command.Parameters.Add(dbParameter);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var itemType = typeof(TEntity);
                        var constructor = itemType.GetConstructor(new Type[] { });
                        var instance = constructor.Invoke(new object[] { });
                        var properties = itemType.GetProperties();

                        foreach (var property in properties)
                        {
                            if (!reader.IsDBNull(property.Name))
                                property.SetValue(instance, reader[property.Name]);
                        }

                        items.Add((TEntity)instance);
                    }
                }

                totalCount = (int?)command.Parameters["TotalCount"].Value;
                filteredCount = (int?)command.Parameters["FilteredCount"].Value;
            }

            return (items, totalCount ?? 0, filteredCount ?? 0);
        }

        public (IList<dynamic> Items, int Total, int TotalFilter) GetFromSql(string sql, IList<(string Key, object Value, bool IsOut)> parameters)
        {
            var items = new List<dynamic>();
            int? totalCount = 0;
            int? filteredCount = 0;

            using (var command = _dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.StoredProcedure;
                if (command.Connection.State != ConnectionState.Open) { command.Connection.Open(); }

                foreach (var param in parameters)
                {
                    DbParameter dbParameter = command.CreateParameter();
                    dbParameter.ParameterName = param.Key;
                    if (!param.IsOut)
                    {
                        dbParameter.Value = param.Value;
                    }
                    else
                    {
                        dbParameter.Direction = ParameterDirection.Output;
                        dbParameter.DbType = DbType.Int32;
                    }
                    command.Parameters.Add(dbParameter);
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new ExpandoObject() as IDictionary<string, object>;
                        for (var count = 0; count < reader.FieldCount; count++)
                        {
                            item.Add(reader.GetName(count), reader[count]);
                        }
                        items.Add(item);
                    }
                }

                totalCount = (int?)command.Parameters["TotalCount"].Value;
                filteredCount = (int?)command.Parameters["FilteredCount"].Value;
            }

            return (items, totalCount ?? 0, filteredCount ?? 0);
        }
        #endregion
    }
}