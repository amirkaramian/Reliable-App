using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Domain.Identity.Events;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Shared.DTOs.Identity;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Infrastructure.Identity.Services;

public class AdminGroupService : IAdminGroupService
{
    private readonly IStringLocalizer<AdminGroupService> _localizer;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepositoryAsync _repository;
    private readonly IUserService _userService;

    public AdminGroupService()
    {
    }

    public AdminGroupService(IRepositoryAsync repository, IUserService userService, IStringLocalizer<AdminGroupService> localizer, UserManager<ApplicationUser> userManager)
    {
        _repository = repository;
        _localizer = localizer;
        _userManager = userManager;
        _userService = userService;
    }

    public async Task<Result<Guid>> CreateAdminGroupAsync(CreateAdminGroupRequest request)
    {
        bool moduleExists = await _repository.ExistsAsync<AdminGroup>(a => a.GroupName == request.GroupName && a.Tenant == request.Tenant);
        if (moduleExists) throw new EntityAlreadyExistsException(string.Format(_localizer["groupname.alreadyexists"], request.GroupName));

        if (request.IsSuperAdmin)
        {
            bool moduleSuperAdminExists = await _repository.ExistsAsync<AdminGroup>(a => a.IsSuperAdmin == true && a.Tenant == request.Tenant);
            if (moduleSuperAdminExists) throw new EntityAlreadyExistsException(string.Format(_localizer["groupname.superadmingroupalreadyexists"], request.GroupName));

        }

        var module = new AdminGroup(request.GroupName, request.Status, request.IsDefault, request.IsSuperAdmin, request.Tenant);
        module.DomainEvents.Add(new AdminGroupCreatedEvent(module));
        var moduleId = await _repository.CreateAsync<AdminGroup>((AdminGroup)module);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(moduleId);
    }

    public async Task<Result<Guid>> DeleteAdminGroupAsync(Guid id)
    {
        var moduleToDelete = await _repository.RemoveByIdAsync<AdminGroup>(id);

        if (moduleToDelete == null)
            throw new EntityNotFoundException(string.Format(_localizer["defaultadmingroup.cannotfindthedefaultgroup"], id));

        if (moduleToDelete.IsSuperAdmin)
            throw new InvalidOperationException(string.Format(_localizer["defaultadmingroup.cannotdeletethesuperadmingroup"], id));
        if (moduleToDelete.IsDefault)
            throw new EntityNotFoundException(string.Format(_localizer["defaultadmingroup.cannotdeletethedefaultgroup"], id));

        moduleToDelete.DomainEvents.Add(new AdminGroupDeletedEvent(moduleToDelete));

        if (await _repository.SaveChangesAsync() > 0)
        {
            var defaultGroup = await _repository.FirstByConditionAsync<AdminGroup>(s => s.IsDefault == true);

            var users = await _userManager.Users.AsNoTracking()
               .Where(u => u.AdminGroupId == id.ToString() && u.IsDeleted == false).ToListAsync();
            foreach (var user in users)
            {
                user.AdminGroupId = defaultGroup.Id.ToString();
                await _userManager.UpdateAsync(user);
            }
        }

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<AdminGroupDto>> SearchAsync(AdminGroupListFilter filter)
    {
        var admins = await _repository.GetSearchResultsAsync<AdminGroup, AdminGroupDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
        foreach (var item in admins.Data)
        {
            item.UserCount = await _userManager.Users.AsNoTracking()
               .CountAsync(u => u.AdminGroupId == item.Id.ToString() && u.IsDeleted == false);
        }

        return admins;
    }

    public async Task<Result<Guid>> UpdateAdminGroupAsync(UpdateAdminGroupRequest request, Guid id)
    {
        var module = await _repository.GetByIdAsync<AdminGroup>(id);
        if (module == null) throw new EntityNotFoundException(string.Format(_localizer["admingroup.notfound"], id));
        if (!module.IsSuperAdmin && request.IsSuperAdmin)
        {
            bool moduleSuperAdminExists = await _repository.ExistsAsync<AdminGroup>(a => a.IsSuperAdmin == true && a.Tenant == request.Tenant);
            if (moduleSuperAdminExists) throw new EntityAlreadyExistsException(string.Format(_localizer["groupname.superadmingroupalreadyexists"], request.GroupName));

            var allUsers = await _userService.GetAllUsersofAdminGroupAsync(module.Id.ToString());
            foreach (var item in allUsers.Data)
            {
                await _userService.AssignRolesAsync(item.Id.ToString(), new UserRolesRequest()
                {
                    UserRoles = new List<UserRoleDto>()
                            {
                                new UserRoleDto()
                                {
                                    RoleName = RoleConstants.SuperAdmin,
                                    Enabled = true
                                }
                            }
                });
            }
        }

        if (module.IsSuperAdmin && !request.IsSuperAdmin)
        {
            var allUsers = await _userService.GetAllUsersofAdminGroupAsync(module.Id.ToString());
            foreach (var item in allUsers.Data)
            {
                await _userService.AssignRolesAsync(item.Id.ToString(), new UserRolesRequest()
                {
                    UserRoles = new List<UserRoleDto>()
                            {
                                new UserRoleDto()
                                {
                                    RoleName = RoleConstants.Admin
                                }
                            }
                });
            }

        }

        var updatedModule = module.Update(request.GroupName, request.Status, request.IsDefault, request.IsSuperAdmin, request.Tenant);
        updatedModule.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<AdminGroup>(updatedModule);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<AdminGroupDto>> GetAdminGroupAsync(Guid id)
    {
        var spec = new BaseSpecification<AdminGroup>();
        var module = await _repository.GetByIdAsync<AdminGroup, AdminGroupDto>(id, spec);

        // module.UserCount = await _userManager.Users.AsNoTracking()
        //   .CountAsync(u => u.AdminGroupId == id.ToString() && u.IsDeleted == false);
        return await Result<AdminGroupDto>.SuccessAsync(module);
    }

    public async Task<Result<AdminGroupDto>> GetDefaultAdminGroupAsync()
    {
        var adminGroup = await _repository.FirstByConditionAsync<AdminGroup>(m => m.IsDefault == true);
        var adminGroupdto = adminGroup.Adapt<AdminGroupDto>();

        // adminGroupdto.UserCount = await _userManager.Users.AsNoTracking()
        //   .CountAsync(u => u.AdminGroupId == adminGroupdto.Id.ToString() && u.IsDeleted == false);
        return await Result<AdminGroupDto>.SuccessAsync(adminGroupdto);
    }

    public async Task<Result<AdminGroupDto>> GetSuperAdminGroupAsync()
    {
        var adminGroup = await _repository.FirstByConditionAsync<AdminGroup>(m => m.IsSuperAdmin == true);
        var adminGroupdto = adminGroup.Adapt<AdminGroupDto>();

        // adminGroupdto.UserCount = await _userManager.Users.AsNoTracking()
        //   .CountAsync(u => u.AdminGroupId == adminGroupdto.Id.ToString() && u.IsDeleted == false);
        return await Result<AdminGroupDto>.SuccessAsync(adminGroupdto);

    }
}