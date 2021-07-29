using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Linq.Expressions;
using UltraPro.Services.Interfaces;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Common.Enums;
using UltraPro.Repositories.Extensions;
using UltraPro.Common.Exceptions;
using UltraPro.Common.Constants;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Query;
using UltraPro.Common.Helpers;
using UltraPro.Common.Services;
using UltraPro.Common.Model;

namespace UltraPro.Services.Implements
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public ApplicationUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ICurrentUserService currentUserService,
            IDateTime dateTime
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<QueryResult<ApplicationUser>> GetAllAsync(UserQuery queryObj)
        {
            var result = new QueryResult<ApplicationUser>();

            var columnsMap = new Dictionary<string, Expression<Func<ApplicationUser, object>>>()
            {
                ["fullName"] = v => v.FullName,
                ["userName"] = v => v.UserName,
                ["email"] = v => v.Email
            };

            var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

            result.Total = await query.CountAsync();

            query = query.Where(x => !x.IsDeleted &&
                x.Status != EnumApplicationUserStatus.SuperAdmin &&
                (string.IsNullOrWhiteSpace(queryObj.FullName) || x.FullName.Contains(queryObj.FullName)) &&
                (string.IsNullOrWhiteSpace(queryObj.UserName) || x.UserName.Contains(queryObj.UserName)) &&
                (string.IsNullOrWhiteSpace(queryObj.PhoneNumber) || x.UserName.Contains(queryObj.PhoneNumber)) &&
                (string.IsNullOrWhiteSpace(queryObj.Email) || x.Email.Contains(queryObj.Email)));

            result.TotalFilter = await query.CountAsync();
            query = query.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending);
            query = query.ApplyPaging(queryObj.Page, queryObj.PageSize);
            result.Items = (await query.AsNoTracking().ToListAsync());

            return result;
        }

        public async Task<QueryResult<ApplicationUser>> GetAllExceptAppUsersAsync(UserQuery queryObj)
        {
            var result = new QueryResult<ApplicationUser>();

            var columnsMap = new Dictionary<string, Expression<Func<ApplicationUser, object>>>()
            {
                ["fullName"] = v => v.FullName,
                ["userName"] = v => v.UserName,
                ["email"] = v => v.Email,
                ["phoneNumber"] = v => v.PhoneNumber
            };

            var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();
            result.Total = await query.CountAsync();

            query = query.Where(x => !x.IsDeleted &&
                x.Status != EnumApplicationUserStatus.SuperAdmin &&
                !x.UserRoles.Any(ur => ur.Role.Name == ConstantsUserRole.AppUser) &&
                (string.IsNullOrWhiteSpace(queryObj.FullName) || x.FullName.Contains(queryObj.FullName)) &&
                (string.IsNullOrWhiteSpace(queryObj.UserName) || x.UserName.Contains(queryObj.UserName)) &&
                (string.IsNullOrWhiteSpace(queryObj.PhoneNumber) || x.UserName.Contains(queryObj.PhoneNumber)) &&
                (string.IsNullOrWhiteSpace(queryObj.Email) || x.Email.Contains(queryObj.Email)));

            result.TotalFilter = await query.CountAsync();
            query = query.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending);
            query = query.ApplyPaging(queryObj.Page, queryObj.PageSize);
            result.Items = (await query.AsNoTracking().ToListAsync());

            return result;
        }

        public async Task<QueryResult<ApplicationUser>> GetAllAppUsersAsync(UserQuery queryObj)
        {
            var result = new QueryResult<ApplicationUser>();

            var columnsMap = new Dictionary<string, Expression<Func<ApplicationUser, object>>>()
            {
                ["fullName"] = v => v.FullName,
                ["userName"] = v => v.UserName,
                ["email"] = v => v.Email,
                ["phoneNumber"] = v => v.PhoneNumber
            };

            var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();
            result.Total = await query.CountAsync();

            query = query.Where(x => !x.IsDeleted &&
                x.Status != EnumApplicationUserStatus.SuperAdmin &&
                x.UserRoles.Any(ur => ur.Role.Name == ConstantsUserRole.AppUser) &&
                (string.IsNullOrWhiteSpace(queryObj.FullName) || x.FullName.Contains(queryObj.FullName)) &&
                (string.IsNullOrWhiteSpace(queryObj.UserName) || x.UserName.Contains(queryObj.UserName)) &&
                (string.IsNullOrWhiteSpace(queryObj.PhoneNumber) || x.UserName.Contains(queryObj.PhoneNumber)) &&
                (string.IsNullOrWhiteSpace(queryObj.Email) || x.Email.Contains(queryObj.Email)));

            result.TotalFilter = await query.CountAsync();
            query = query.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending);
            query = query.ApplyPaging(queryObj.Page, queryObj.PageSize);
            result.Items = (await query.AsNoTracking().ToListAsync());

            return result;
        }

        public async Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

            var user = await query.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new NotFoundException(nameof(ApplicationUser), id);
            }

            return user;
        }

        public async Task<ApplicationUser> GetByUserNameAsync(string userName)
        {
            var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

            var user = await query.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new NotFoundException(nameof(ApplicationUser), userName);
            }

            return user;
        }

        public async Task<Guid> AddAsync(ApplicationUser entity, Guid userRoleId, string newPassword)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var isExists = await this.IsExistsUserNameAsync(entity.UserName, entity.Id);
                    if (isExists)
                    {
                        throw new DuplicationException(nameof(entity.UserName));
                    }

                    entity.Status = EnumApplicationUserStatus.GeneralUser;
                    entity.Created = _dateTime.Now;
                    entity.CreatedBy = _currentUserService.UserId;

                    var userSaveResult = await _userManager.CreateAsync(entity, newPassword);

                    if (!userSaveResult.Succeeded)
                    {
                        throw new IdentityValidationException(userSaveResult.Errors);
                    };

                    // Add New User Role
                    var user = await _userManager.FindByNameAsync(entity.UserName);
                    var role = await _roleManager.FindByIdAsync(userRoleId.ToString());

                    if (role == null)
                    {
                        throw new NotFoundException(nameof(ApplicationRole), userRoleId);
                    }

                    var roleSaveResult = await _userManager.AddToRoleAsync(user, role.Name);

                    if (!roleSaveResult.Succeeded)
                    {
                        throw new IdentityValidationException(roleSaveResult.Errors);
                    };

                    scope.Complete();

                    return user.Id;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<Guid> UpdateAsync(ApplicationUser entity, Guid userRoleId)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var user = await this._userManager.FindByIdAsync(entity.Id.ToString());

                    if (user == null)
                    {
                        throw new NotFoundException(nameof(ApplicationUser), entity.Id);
                    }

                    var isExists = await this.IsExistsUserNameAsync(entity.UserName, entity.Id);
                    if (isExists)
                    {
                        throw new DuplicationException(nameof(entity.Email));
                    }

                    user.FullName = entity.FullName;
                    user.UserName = entity.UserName;
                    user.Email = entity.Email;
                    user.PhoneNumber = entity.PhoneNumber;
                    user.Address = entity.Address;
                    user.DateOfBirth = entity.DateOfBirth;
                    user.Gender = entity.Gender;
                    user.ImageUrl = entity.ImageUrl ?? user.ImageUrl;
                    user.LastModified = _dateTime.Now;
                    user.LastModifiedBy = _currentUserService.UserId;

                    var userSaveResult = await _userManager.UpdateAsync(user);

                    if (!userSaveResult.Succeeded)
                    {
                        throw new IdentityValidationException(userSaveResult.Errors);
                    };

                    // Remove Previous User Role
                    var previousUserRoles = await _userManager.GetRolesAsync(user);
                    if (previousUserRoles.Any())
                    {
                        var roleRemoveResult = await _userManager.RemoveFromRolesAsync(user, previousUserRoles);

                        if (!roleRemoveResult.Succeeded)
                        {
                            throw new IdentityValidationException(roleRemoveResult.Errors);
                        };

                    }

                    // Add New User Role
                    var role = await _roleManager.FindByIdAsync(userRoleId.ToString());

                    if (role == null)
                    {
                        throw new NotFoundException(nameof(ApplicationRole), userRoleId);
                    }

                    var roleSaveResult = await _userManager.AddToRoleAsync(user, role.Name);

                    if (!roleSaveResult.Succeeded)
                    {
                        throw new IdentityValidationException(roleSaveResult.Errors);
                    };

                    scope.Complete();

                    return user.Id;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id.ToString());

                    if (user == null)
                    {
                        throw new NotFoundException(nameof(ApplicationUser), id);
                    }

                    //user.IsDeleted = true;
                    //var result = await _userManager.UpdateAsync(user);
                    var result = await _userManager.DeleteAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new IdentityValidationException(result.Errors);
                    };

                    scope.Complete();

                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<bool> ActiveInactiveAsync(Guid id)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id.ToString());

                    if (user == null)
                    {
                        throw new NotFoundException(nameof(ApplicationUser), id);
                    }

                    user.IsActive = !user.IsActive;
                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        throw new IdentityValidationException(result.Errors);
                    };

                    scope.Complete();

                    return true;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<IList<KeyValuePairObject>> GetAllForSelectAsync()
        {
            return await _userManager.Users.Where(x => x.IsActive && !x.IsDeleted && x.Status != EnumApplicationUserStatus.SuperAdmin).OrderBy(x => x.FullName)
                                            .Select(s => new KeyValuePairObject { Value = s.Id, Text = s.FullName }).ToListAsync();
        }

        public async Task<IList<KeyValuePairObject>> GetAllExceptAppUsersForSelectAsync()
        {
            return await _userManager.Users.Where(x => x.IsActive && !x.IsDeleted && x.Status != EnumApplicationUserStatus.SuperAdmin &&
                                                !x.UserRoles.Any(ur => ur.Role.Name == ConstantsUserRole.AppUser)).OrderBy(x => x.FullName)
                                            .Select(s => new KeyValuePairObject { Value = s.Id, Text = s.FullName }).ToListAsync();
        }

        public async Task<IList<KeyValuePairObject>> GetAllAppUsersForSelectAsync()
        {
            return await _userManager.Users.Where(x => x.IsActive && !x.IsDeleted && x.Status != EnumApplicationUserStatus.SuperAdmin &&
                                                x.UserRoles.Any(ur => ur.Role.Name == ConstantsUserRole.AppUser)).OrderBy(x => x.FullName)
                                            .Select(s => new KeyValuePairObject { Value = s.Id, Text = s.FullName }).ToListAsync();
        }

        public async Task<bool> IsExistsUserNameAsync(string name, Guid id)
        {
            var result = await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == name.ToLower() && x.Id != id && !x.IsDeleted);
            return result;
        }

        public async Task<bool> IsExistsEmailAsync(string email, Guid id)
        {
            var result = await _userManager.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower() && x.Id != id && !x.IsDeleted);
            return result;
        }

        #region Helper
        public virtual async Task<IList<TResult>> GetAsync<TResult>(Expression<Func<ApplicationUser, TResult>> selector,
                            Expression<Func<ApplicationUser, bool>> predicate = null,
                            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderBy = null,
                            Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>> include = null,
                            bool disableTracking = true)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.AsQueryable();

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

        public virtual async Task<(IList<TResult> Items, int Total, int TotalFilter)> GetAsync<TResult>(Expression<Func<ApplicationUser, TResult>> selector,
                            Expression<Func<ApplicationUser, bool>> predicate = null,
                            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderBy = null,
                            Func<IQueryable<ApplicationUser>, IIncludableQueryable<ApplicationUser, object>> include = null,
                            int pageIndex = 1, int pageSize = 10,
                            bool disableTracking = true)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.AsQueryable();

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

        public virtual async Task<int> GetCountAsync(Expression<Func<ApplicationUser, bool>> predicate = null)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.CountAsync();
        }

        public virtual async Task<long> GetSumAsync(Expression<Func<ApplicationUser, long>> selector, Expression<Func<ApplicationUser, bool>> predicate = null)
        {
            IQueryable<ApplicationUser> query = _userManager.Users.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.SumAsync(selector);
        }
        #endregion
    }
}
