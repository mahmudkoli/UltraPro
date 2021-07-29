using UltraPro.Common.Constants;
using UltraPro.Common.Enums;
using UltraPro.Entities;
using UltraPro.Entities.NotMapped;
using UltraPro.Repositories.Extensions;
using UltraPro.Common.Exceptions;
using UltraPro.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Transactions;
using UltraPro.Common.Services;
using UltraPro.Common.Model;

namespace UltraPro.Services.Implements
{
    public class ApplicationRoleService : IApplicationRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public ApplicationRoleService(
            RoleManager<ApplicationRole> roleManager,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<QueryResult<ApplicationRole>> GetAllAsync(UserRoleQuery queryObj)
        {
            var result = new QueryResult<ApplicationRole>();

            var columnsMap = new Dictionary<string, Expression<Func<ApplicationRole, object>>>()
            {
                ["name"] = v => v.Name,
            };

            var query = _roleManager.Roles.AsQueryable();
            result.Total = await query.CountAsync();

            query = query.Where(x => !x.IsDeleted &&
                x.Status != EnumApplicationRoleStatus.SuperAdmin &&
                (string.IsNullOrWhiteSpace(queryObj.Name) || x.Name.Contains(queryObj.Name)));

            result.TotalFilter = await query.CountAsync();
            query = query.ApplyOrdering(columnsMap, queryObj.SortBy, queryObj.IsSortAscending);
            query = query.ApplyPaging(queryObj.Page, queryObj.PageSize);
            result.Items = (await query.AsNoTracking().ToListAsync());

            return result;
        }

        public async Task<(ApplicationRole ApplicationRole, IList<string> Permissions)> GetByIdAsync(Guid id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());

            if (role == null)
            {
                throw new NotFoundException(nameof(ApplicationRole), id);
            }

            var permissions = (await _roleManager.GetClaimsAsync(role)).Select(c => c.Value).ToList();

            return (role, permissions);
        }

        public async Task<(ApplicationRole ApplicationRole, IList<string> Permissions)> GetByNameAsync(string name)
        {
            var role = await _roleManager.FindByNameAsync(name);

            if (role == null)
            {
                throw new NotFoundException(nameof(ApplicationRole), name);
            }

            var permissions = (await _roleManager.GetClaimsAsync(role)).Select(c => c.Value).ToList();

            return (role, permissions);
        }

        public async Task<Guid> AddAsync(ApplicationRole entity, IList<string> permissions)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var isExists = await this.IsExistsNameAsync(entity.Name, entity.Id);
                    if (isExists)
                    {
                        throw new DuplicationException(nameof(entity.Name));
                    }

                    entity.Status = EnumApplicationRoleStatus.GeneralUser;
                    entity.Created = _dateTime.Now;
                    entity.CreatedBy = _currentUserService.UserId;

                    var roleSaveResult = await _roleManager.CreateAsync(entity);

                    if (!roleSaveResult.Succeeded)
                    {
                        throw new IdentityValidationException(roleSaveResult.Errors);
                    };

                    var role = await _roleManager.FindByNameAsync(entity.Name);

                    foreach (var per in permissions)
                    {
                        var claimSaveResult = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimType.Permission, per));

                        if (!claimSaveResult.Succeeded)
                        {
                            throw new IdentityValidationException(claimSaveResult.Errors);
                        };

                    }

                    scope.Complete();

                    return role.Id;
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw;
                }
            }
        }

        public async Task<Guid> UpdateAsync(ApplicationRole entity, IList<string> permissions)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var role = await _roleManager.FindByIdAsync(entity.Id.ToString());

                    if (role == null)
                    {
                        throw new NotFoundException(nameof(ApplicationRole), entity.Id);
                    }

                    var isExists = await this.IsExistsNameAsync(entity.Name, entity.Id);
                    if (isExists)
                    {
                        throw new DuplicationException(nameof(entity.Name));
                    }

                    role.Name = entity.Name;
                    role.LastModified = _dateTime.Now;
                    role.LastModifiedBy = _currentUserService.UserId;

                    var roleSaveResult = await this._roleManager.UpdateAsync(role);

                    if (!roleSaveResult.Succeeded)
                    {
                        throw new IdentityValidationException(roleSaveResult.Errors);
                    };

                    // Remove Previous Permission
                    var removedPermissions = await _roleManager.GetClaimsAsync(role);
                    foreach (var per in removedPermissions)
                    {
                        var claimRemoveResult = await _roleManager.RemoveClaimAsync(role, per);

                        if (!claimRemoveResult.Succeeded)
                        {
                            throw new IdentityValidationException(claimRemoveResult.Errors);
                        };

                    }

                    // Add New Permission
                    foreach (var per in permissions)
                    {
                        var claimSaveResult = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimType.Permission, per));

                        if (!claimSaveResult.Succeeded)
                        {
                            throw new IdentityValidationException(claimSaveResult.Errors);
                        };

                    }

                    scope.Complete();

                    return role.Id;
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
                    var role = await _roleManager.FindByIdAsync(id.ToString());

                    if (role == null)
                    {
                        throw new NotFoundException(nameof(ApplicationRole), id);
                    }

                    //role.IsDeleted = true;
                    //var result = await _roleManager.UpdateAsync(role);
                    var result = await _roleManager.DeleteAsync(role);

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
                    var role = await _roleManager.FindByIdAsync(id.ToString());

                    if (role == null)
                    {
                        throw new NotFoundException(nameof(ApplicationRole), id);
                    }

                    role.IsActive = !role.IsActive;
                    var result = await _roleManager.UpdateAsync(role);

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
            return await _roleManager.Roles.Where(x => x.IsActive && !x.IsDeleted && x.Status != EnumApplicationRoleStatus.SuperAdmin &&
                                                x.Name != ConstantsUserRole.AppUser).OrderBy(x => x.Name)
                                            .Select(x => new KeyValuePairObject { Value = x.Id, Text = x.Name }).ToListAsync();
        }
        
        public async Task<bool> IsExistsNameAsync(string name, Guid id)
        {
            var result = await _roleManager.Roles.AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.Id != id && !x.IsDeleted);
            return result;
        }
    }
}
