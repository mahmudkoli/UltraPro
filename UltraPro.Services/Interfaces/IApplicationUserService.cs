using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UltraPro.Common.Model;

namespace UltraPro.Services.Interfaces
{
    public interface IApplicationUserService
    {
        Task<QueryResult<ApplicationUser>> GetAllAsync(UserQuery queryObj);
        Task<QueryResult<ApplicationUser>> GetAllExceptAppUsersAsync(UserQuery queryObj);
        Task<QueryResult<ApplicationUser>> GetAllAppUsersAsync(UserQuery queryObj);
        Task<ApplicationUser> GetByIdAsync(Guid id);
        Task<ApplicationUser> GetByUserNameAsync(string userName);
        Task<Guid> AddAsync(ApplicationUser command, Guid userRoleId, string newPassword);
        Task<Guid> UpdateAsync(ApplicationUser command, Guid userRoleId);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ActiveInactiveAsync(Guid id);
        Task<IList<KeyValuePairObject>> GetAllForSelectAsync();
        Task<IList<KeyValuePairObject>> GetAllExceptAppUsersForSelectAsync();
        Task<IList<KeyValuePairObject>> GetAllAppUsersForSelectAsync();
        Task<bool> IsExistsUserNameAsync(string name, Guid id);
        Task<bool> IsExistsEmailAsync(string email, Guid id);

        #region Helper
        Task<IList<TResult>> GetAsync<TResult>(Expression<Func<ApplicationUser, TResult>> selector,
                            Expression<Func<ApplicationUser, bool>> predicate = null,
                            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderBy = null,
                            Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>> include = null,
                            bool disableTracking = true);
        Task<(IList<TResult> Items, int Total, int TotalFilter)> GetAsync<TResult>(Expression<Func<ApplicationUser, TResult>> selector,
                            Expression<Func<ApplicationUser, bool>> predicate = null,
                            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderBy = null,
                            Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>> include = null,
                            int pageIndex = 1, int pageSize = 10,
                            bool disableTracking = true);
        Task<int> GetCountAsync(Expression<Func<ApplicationUser, bool>> predicate = null);
        Task<long> GetSumAsync(Expression<Func<ApplicationUser, long>> selector, Expression<Func<ApplicationUser, bool>> predicate = null);
        #endregion
    }
}
