using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Identity.Exceptions;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Multitenancy;
using MyReliableSite.Application.Settings;
using MyReliableSite.Application.Specifications;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Domain.Identity;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Domain.ManageModule.Events;
using MyReliableSite.Domain.ManageUserApiKey;
using MyReliableSite.Domain.Multitenancy;
using MyReliableSite.Infrastructure.Common.Extensions;
using MyReliableSite.Infrastructure.Common.Services;
using MyReliableSite.Infrastructure.Hubs;
using MyReliableSite.Infrastructure.Identity.Extensions;
using MyReliableSite.Infrastructure.Identity.Models;
using MyReliableSite.Infrastructure.Persistence.Contexts;
using MyReliableSite.Infrastructure.Persistence.Converters;
using MyReliableSite.Infrastructure.Persistence.Repositories;
using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.General.Requests;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.ManageModule;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Tickets;
using Org.BouncyCastle.Math.EC.Rfc7748;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IStringLocalizer<UserService> _localizer;
    private readonly ICurrentUser _user;
    private readonly ApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IRepositoryAsync _repository;
    private readonly ITenantService _tenantService;
    private readonly MailSettings _mailSettings;
    private readonly IJobService _jobService;
    private readonly IEmailTemplateService _templateService;
    private readonly IMailService _mailService;
    private readonly IHubContext<NotificationHub> _notificationHubContext;

    public string CurrentTenant { get; set; }
    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IStringLocalizer<UserService> localizer,
        ApplicationDbContext context,
        ICurrentUser user,
        ITokenService tokenService,
        IFileStorageService fileStorageService,
        IRepositoryAsync repository,
        ITenantService tenantService,
        IOptions<MailSettings> mailSettings,
        IJobService jobService,
        IMailService mailService,
        IEmailTemplateService templateService,
        IHubContext<NotificationHub> notificationHubContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _localizer = localizer;
        _context = context;
        _user = user;
        _tokenService = tokenService;
        _fileStorageService = fileStorageService;
        _repository = repository;
        _tenantService = tenantService;
        _mailSettings = mailSettings.Value;
        _jobService = jobService;
        _mailService = mailService;
        _templateService = templateService;
        _notificationHubContext = notificationHubContext;
        CurrentTenant = _tenantService?.GetCurrentTenant()?.Key;

    }

    public UserService()
    {
    }

    public async Task<PaginatedResult<UserDetailsDto>> SearchAsync(UserListFilter filter)
    {
        var filters = new Filters<ApplicationUser>();
        filters.Add(filter.IsActive.HasValue, x => x.IsActive == filter.IsActive);

        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            filters.Add(true, x => x.Tenant == CurrentTenant);
        }

        var query = _userManager.Users.ApplyFilter(filters).AdvancedSearch(filter.AdvancedSearch);
        string ordering = new OrderByConverter().ConvertBack(filter.OrderBy);
        query = !string.IsNullOrWhiteSpace(ordering) ? query.OrderBy(ordering) : query.OrderBy(a => a.Id);

        return await query.ToMappedPaginatedResultAsync<ApplicationUser, UserDetailsDto>(filter.PageNumber, filter.PageSize);
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllAsync(IEnumerable<string> ids)
    {
        if (ids == null) return await Result<List<UserDetailsDto>>.SuccessAsync(new List<UserDetailsDto>());

        ids = ids.Select(x => x.ToLower());

        var users = await _userManager.Users.AsNoTracking().Where(x => ids.Contains(x.Id.ToLower()) && x.IsDeleted == false).ToListAsync();
        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            users = users.Where(u => u.Tenant.ToLower() == CurrentTenant.ToLower()).ToList();
        }

        var result = users.Adapt<List<UserDetailsDto>>();
        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllAsync()
    {
        var users = await _userManager.Users.AsNoTracking().Where(x => x.IsDeleted == false).ToListAsync();
        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            users = users.Where(u => u.Tenant.ToLower() == CurrentTenant.ToLower()).ToList();
        }

        var result = users.Adapt<List<UserDetailsDto>>();
        if (result != null)
        {
            foreach (var item in result)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.Id.ToString());

                item.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
            }
        }

        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<Result<List<UserDetailsEXL>>> GetSubUserListAsync(string userId, DateTime startDate, DateTime endDate)
    {

        var users = await _userManager.Users.AsNoTracking().Where(x => x.IsDeleted == false && x.ParentID == userId && x.CreatedOn >= startDate && x.CreatedOn <= endDate).ToListAsync();
        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            users = users.Where(u => u.Tenant.ToLower() == CurrentTenant.ToLower()).ToList();
        }

        var result = users.Adapt<List<UserDetailsEXL>>();
        if (result != null)
        {
            foreach (var item in result)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.Id.ToString());

                item.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
            }
        }

        return await Result<List<UserDetailsEXL>>.SuccessAsync(result);
    }

    public async Task<string> GenerateUserToken(string purpose = "General")
    {
        string provider = TokenOptions.DefaultProvider;
        var user = await _userManager.FindByIdAsync(_user.GetUserId().ToString());
        return await _userManager.GenerateUserTokenAsync(user, provider, purpose);
    }

    public async Task<bool> VerifyUserToken(string token, string purpose = "General")
    {
        string provider = TokenOptions.DefaultProvider;
        var user = await _userManager.FindByIdAsync(_user.GetUserId().ToString());
        return await _userManager.VerifyUserTokenAsync(user, provider, purpose, token);
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllSubUsersAsync()
    {
        var users = await _userManager.Users.AsNoTracking().Where(x => x.IsDeleted == false && !string.IsNullOrEmpty(x.ParentID)).ToListAsync();
        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            users = users.Where(u => u.Tenant.ToLower() == CurrentTenant.ToLower()).ToList();
        }

        var result = users.Adapt<List<UserDetailsDto>>();
        if (result != null)
        {
            foreach (var item in result)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.Id.ToString());

                item.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
            }
        }

        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllSubUsersByClientIDAsync(string clientId)
    {
        var users = await _userManager.Users.AsNoTracking().Where(x => x.IsDeleted == false && x.ParentID == clientId).ToListAsync();
        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            users = users.Where(u => u.Tenant.ToLower() == CurrentTenant.ToLower()).ToList();
        }

        var result = users.Adapt<List<UserDetailsDto>>();
        if (result != null)
        {
            foreach (var item in result)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.Id.ToString());

                item.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
            }
        }

        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<PaginatedResult<UserDetailsDto>> SearchSubUsersAsync(UserListFilter filter)
    {
        var filters = new Filters<ApplicationUser>();
        filters.Add(filter.IsActive.HasValue, x => x.IsActive == filter.IsActive);

        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            filters.Add(true, x => x.Tenant == CurrentTenant);
        }

        var query = _userManager.Users.Where(x => !string.IsNullOrEmpty(x.ParentID)).ApplyFilter(filters).AdvancedSearch(filter.AdvancedSearch);
        string ordering = new OrderByConverter().ConvertBack(filter.OrderBy);
        query = !string.IsNullOrWhiteSpace(ordering) ? query.OrderBy(ordering) : query.OrderBy(a => a.Id);

        return await query.ToMappedPaginatedResultAsync<ApplicationUser, UserDetailsDto>(filter.PageNumber, filter.PageSize);
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllByUserRoleAsync(string roleName)
    {
        var usersOfRole = await _userManager.GetUsersInRoleAsync(roleName);
        var result = usersOfRole.Adapt<List<UserDetailsDto>>();
        if (result != null)
        {
            foreach (var item in result)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.Id.ToString());

                item.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
                var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == item.Id);
                if (departments != null && departments.Count() > 0)
                {
                    item.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

                }

                var settings = await _repository.FindByConditionAsync<UserAppSetting>(x => x.UserId == item.Id.ToString());
                if (settings != null && settings.Any())
                {
                    var setting = settings.First();
                    item.AutoAssignOrders = setting.AutoAssignOrders;
                    item.CanTakeOrders = setting.CanTakeOrders;
                    item.AvaillableForOrders = setting.AvaillableForOrders;
                }
            }
        }

        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllUsersToTakeOrdersAsync()
    {
        var users = await _userManager.Users.AsNoTracking().Where(x => !x.IsDeleted && x.CanTakeOrders).ToListAsync();
        var result = users.Adapt<List<UserDetailsDto>>();
        if (result != null)
        {
            foreach (var item in result)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.Id.ToString());
                var settings = await _repository.FindByConditionAsync<UserAppSetting>(x => x.UserId == item.Id.ToString());
                if (settings != null && settings.Any())
                {
                    var setting = settings.First();
                    item.AutoAssignOrders = setting.AutoAssignOrders;
                    item.CanTakeOrders = setting.CanTakeOrders;
                    item.AvaillableForOrders = setting.AvaillableForOrders;
                }

                item.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
                var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == item.Id);
                if (departments != null && departments.Count() > 0)
                {
                    item.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

                }
            }
        }

        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetAsync(string userId)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == userId && u.IsDeleted == false).FirstOrDefaultAsync();
        var result = user.Adapt<UserDetailsDto>();
        if (result != null)
        {
            if (!string.IsNullOrEmpty(result.ImageUrl))
            {
                result.Base64Image = result.ImageUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(result.ImageUrl);
            }

            var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == Guid.Parse(user.Id));
            if (departments != null && departments.Count() > 0)
            {
                result.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

            }

            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            result.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
        }

        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetSubUserAsync(string userId)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == userId && u.IsDeleted == false && !string.IsNullOrEmpty(u.ParentID)).FirstOrDefaultAsync();
        var result = user.Adapt<UserDetailsDto>();
        if (result != null)
        {
            if (!string.IsNullOrEmpty(result.ImageUrl))
            {
                result.Base64Image = result.ImageUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(result.ImageUrl);
            }

            var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == Guid.Parse(user.Id));
            if (departments != null && departments.Count() > 0)
            {
                result.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

            }

            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            result.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

            var userModules = await _repository.FindByConditionAsync<UserModule>(m => m.UserId == user.Id);
            if (userModules != null && userModules.Count() > 0)
            {
                result.UserModules = userModules.Adapt<List<UserModuleDto>>();
            }
        }

        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetUserProfileAsync()
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == _user.GetUserId().ToString() && u.IsDeleted == false).FirstOrDefaultAsync();
        var result = user.Adapt<UserDetailsDto>();
        if (result != null)
        {
            var userRole = await GetRolesAsync(_user.GetUserId().ToString());
            result.UserRolesResponse = userRole.Data;
            if (!string.IsNullOrEmpty(result.ImageUrl))
            {
                result.Base64Image = result.ImageUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(result.ImageUrl);
            }

            var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == Guid.Parse(user.Id));
            if (departments != null && departments.Count() > 0)
            {
                result.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

            }

            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            result.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();
        }

        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetUserProfileAsync(string id)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == id && u.IsDeleted == false).FirstOrDefaultAsync();
        var result = user.Adapt<UserDetailsDto>();
        if (result != null && !string.IsNullOrEmpty(result.ImageUrl))
        {
            result.Base64Image = result.ImageUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(result.ImageUrl);
        }

        if (result != null)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            result.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

            var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == Guid.Parse(user.Id));
            if (departments != null && departments.Count() > 0)
            {
                result.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

            }
        }

        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetUserProfileByFullNameAsync(string userFullName, int oldUserId)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => (u.FullName == userFullName || u.OldUserId == oldUserId) && u.IsDeleted == false).FirstOrDefaultAsync();
        var result = user.Adapt<UserDetailsDto>();
        if (result != null && !string.IsNullOrEmpty(result.ImageUrl))
        {
            result.Base64Image = result.ImageUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(result.ImageUrl);
        }

        if (result != null)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            result.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

            var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == Guid.Parse(user.Id));
            if (departments != null && departments.Count() > 0)
            {
                result.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

            }
        }

        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetUserProfileByEmailAsync(string email)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Email == email && u.IsDeleted == false).FirstOrDefaultAsync();
        var result = user.Adapt<UserDetailsDto>();
        if (result != null && !string.IsNullOrEmpty(result.ImageUrl))
        {
            result.Base64Image = result.ImageUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(result.ImageUrl);
        }

        if (result != null)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            result.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

            var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == Guid.Parse(user.Id));
            if (departments != null && departments.Count() > 0)
            {
                result.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();

            }
        }

        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetUserProfileByUsernameAsync(string username)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.UserName == username && u.IsDeleted == false).FirstOrDefaultAsync();
        var result = user.Adapt<UserDetailsDto>();
        if (result != null && !string.IsNullOrEmpty(result.ImageUrl))
        {
            result.Base64Image = result.ImageUrl; // await _fileStorageService.ReturnBase64StringOfImageFileAsync(result.ImageUrl);
        }

        if (result != null)
        {
            var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == user.Id);

            result.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

            var departments = await _repository.FindByConditionAsync<DepartmentAdmin>(m => m.AdminUserId == Guid.Parse(user.Id));
            if (departments != null && departments.Count() > 0)
            {
                result.DepartmentIds = departments.Select(m => m.DepartmentId).ToList();
            }
        }

        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
    {
        var viewModel = new List<UserRoleDto>();
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
        foreach (var role in roles)
        {
            var userRolesViewModel = new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            userRolesViewModel.Enabled = await _userManager.IsInRoleAsync(user, role.Name);

            viewModel.Add(userRolesViewModel);
        }

        var result = new UserRolesResponse { UserRoles = viewModel };
        return await Result<UserRolesResponse>.SuccessAsync(result);
    }

    public async Task<IResult<UserRolesResponse>> GetSubUserRolesAsync(string userId)
    {
        var viewModel = new List<UserRoleDto>();
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == userId && u.IsDeleted == false && !string.IsNullOrEmpty(u.ParentID)).FirstOrDefaultAsync();
        if (user == null)
        {
            return await Result<UserRolesResponse>.FailAsync(_localizer["Sub User Not Found."]);
        }

        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
        foreach (var role in roles)
        {
            var userRolesViewModel = new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            userRolesViewModel.Enabled = await _userManager.IsInRoleAsync(user, role.Name);

            viewModel.Add(userRolesViewModel);
        }

        var result = new UserRolesResponse { UserRoles = viewModel };
        return await Result<UserRolesResponse>.SuccessAsync(result);
    }

    public async Task<IResult<string>> AssignRolesAsync(string userId, UserRolesRequest request)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        if (await _userManager.IsInRoleAsync(user, RoleConstants.Admin) && !request.UserRoles.Any(m => m.RoleName == RoleConstants.SuperAdmin))
        {
            return await Result<string>.FailAsync(_localizer["Not Allowed."]);
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await _roleManager.FindByNameAsync(userRole.RoleName) != null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                }
            }
        }

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User Roles Updated Successfully."]));
    }

    public async Task<IResult<string>> AssignSubUserRolesAsync(string userId, UserRolesRequest request)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId && !string.IsNullOrEmpty(u.ParentID)).FirstOrDefaultAsync();
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["Sub User Not Found."]);
        }

        if (await _userManager.IsInRoleAsync(user, RoleConstants.Admin) && !request.UserRoles.Any(m => m.RoleName == RoleConstants.SuperAdmin))
        {
            return await Result<string>.FailAsync(_localizer["Not Allowed."]);
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await _roleManager.FindByNameAsync(userRole.RoleName) != null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                }
            }
        }

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User Roles Updated Successfully."]));
    }

    public async Task<Result<List<PermissionDto>>> GetPermissionsAsync(string userId)
    {
        var userPermissions = new List<PermissionDto>();
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<List<PermissionDto>>.FailAsync(_localizer["User Not Found."]);
        }

        var roleNames = await _userManager.GetRolesAsync(user);
        foreach (var role in _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToList())
        {
            var permissions = await _context.RoleClaims.Where(a => a.RoleId == role.Id && a.ClaimType == "Permission").ToListAsync();
            var permissionResponse = permissions.Adapt<List<PermissionDto>>();
            userPermissions.AddRange(permissionResponse);
        }

        return await Result<List<PermissionDto>>.SuccessAsync(userPermissions.Distinct().ToList());
    }

    public async Task<Result<List<PermissionDto>>> GetSubUserPermissionsAsync(string userId)
    {
        var userPermissions = new List<PermissionDto>();
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == userId && u.IsDeleted == false && !string.IsNullOrEmpty(u.ParentID)).FirstOrDefaultAsync();
        if (user == null)
        {
            return await Result<List<PermissionDto>>.FailAsync(_localizer["Sub User Not Found."]);
        }

        var roleNames = await _userManager.GetRolesAsync(user);
        foreach (var role in _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToList())
        {
            var permissions = await _context.RoleClaims.Where(a => a.RoleId == role.Id && a.ClaimType == "Permission").ToListAsync();
            var permissionResponse = permissions.Adapt<List<PermissionDto>>();
            userPermissions.AddRange(permissionResponse);
        }

        return await Result<List<PermissionDto>>.SuccessAsync(userPermissions.Distinct().ToList());
    }

    public async Task<int> GetCountAsync()
    {
        if (!string.IsNullOrEmpty(CurrentTenant) && CurrentTenant.ToLower() == "client")
        {
            return await _userManager.Users.Where(x => x.IsDeleted == false && x.Tenant.ToLower() == CurrentTenant.ToLower()).AsNoTracking().CountAsync();
        }
        else
        {
            return await _userManager.Users.Where(x => x.IsDeleted == false).AsNoTracking().CountAsync();
        }
    }

    public async Task<Result<Guid>> DeleteUserAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        user.IsDeleted = true;
        await _userManager.UpdateAsync(user);
        return await Result<Guid>.SuccessAsync(userId);
    }

    public async Task<Result<Guid>> DeleteUserAccountAsync()
    {
        var user = await _userManager.FindByIdAsync(_user.GetUserId().ToString());
        user.IsDeleted = true;
        await _userManager.UpdateAsync(user);
        return await Result<Guid>.SuccessAsync(user.Id);
    }

    public async Task<IResult<string>> DeactiveUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        // var principal = GetPrincipalFromExpiredToken(token);
        // await _userManager.UpdateSecurityStampAsync(user);

        await _tokenService.RevokeToken(userId);
        user.IsActive = false;
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User deactivated Successfully."]));
    }

    public async Task<IResult<string>> ActiveUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        user.IsActive = true;
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User Activated Successfully."]));
    }

    public async Task<IResult<string>> ActivateUserTakeOrder(string userId)
    {
        var currentuser = await _userManager.FindByIdAsync(_user.GetUserId().ToString());
        if (currentuser == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        var currentuseradmingroup = await _repository.GetByIdAsync<AdminGroup>(Guid.Parse(currentuser.AdminGroupId));

        if (currentuseradmingroup == null)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        if (!currentuseradmingroup.IsSuperAdmin)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        user.CanTakeOrders = true;
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User Can Take Order Now."]));
    }

    public async Task<IResult<string>> DeActivateUserTakeOrder(string userId)
    {
        var currentuser = await _userManager.FindByIdAsync(_user.GetUserId().ToString());
        if (currentuser == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        var currentuseradmingroup = await _repository.GetByIdAsync<AdminGroup>(Guid.Parse(currentuser.AdminGroupId));

        if (currentuseradmingroup == null)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        if (!currentuseradmingroup.IsSuperAdmin)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        user.CanTakeOrders = false;
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User Can't Take Order Now."]));
    }

    public async Task<IResult<string>> ActivateUserAvailableForOrders(string userId)
    {
        var currentuser = await _userManager.FindByIdAsync(_user.GetUserId().ToString());
        if (currentuser == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        var currentuseradmingroup = await _repository.GetByIdAsync<AdminGroup>(Guid.Parse(currentuser.AdminGroupId));

        if (currentuseradmingroup == null)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        if (!currentuseradmingroup.IsSuperAdmin)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        user.AvailableForOrders = true;
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User Available Order Now."]));
    }

    public async Task<IResult<string>> DeActivateUserAvailableForOrders(string userId)
    {
        var currentuser = await _userManager.FindByIdAsync(_user.GetUserId().ToString());
        if (currentuser == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        var currentuseradmingroup = await _repository.GetByIdAsync<AdminGroup>(Guid.Parse(currentuser.AdminGroupId));

        if (currentuseradmingroup == null)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        if (!currentuseradmingroup.IsSuperAdmin)
        {
            return await Result<string>.FailAsync(_localizer["Current user is not super admin."]);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["User Not Found."]);
        }

        user.AvailableForOrders = false;
        await _userManager.UpdateAsync(user);

        return await Result<string>.SuccessAsync(userId, string.Format(_localizer["User Is Not Available For Order Now."]));
    }

    public async Task<Result<List<UserDetailsDto>>> GetUsersBasedOnConditionsForNotificationTemplates(UsersBasedOnConditionsRequest usersBasedOnConditionsDto)
    {
        string propertyValue = string.Empty;
        int value = 0;
        bool isParsed = int.TryParse(usersBasedOnConditionsDto.Value, out value);

        if (isParsed)
        {
            propertyValue = @$"{usersBasedOnConditionsDto.Value}";
        }
        else
        {
            propertyValue = @$"'{usersBasedOnConditionsDto.Value}'";
        }

        if (usersBasedOnConditionsDto.Property == ConditionBasedOn.Bills)
        {
            string query = $@"SELECT UserId FROM Bills GROUP BY UserId, SubTotal, VAT HAVING ISNULL(SUM(SubTotal), 0) + ISNULL(VAT, 0) {usersBasedOnConditionsDto.OperatorType} {propertyValue}";
            var bills = await _repository.QueryWithDtoAsync<Bill>(query);
            var userIds = bills.Select(y => y.UserId);

            if (userIds.Any())
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                var billUsersResponse = await _userManager.Users.AsNoTracking().Where(user => userIds.Contains(user.Id) && user.IsDeleted == false).ToListAsync();
                var usersDetail = billUsersResponse.Adapt<List<UserDetailsDto>>();

                return await Result<List<UserDetailsDto>>.SuccessAsync(usersDetail);
            }
        }
        else if (usersBasedOnConditionsDto.Property == ConditionBasedOn.Tickets)
        {
            string query = $@"SELECT AssignedTo FROM Tickets GROUP BY AssignedTo HAVING COUNT(AssignedTo) {usersBasedOnConditionsDto.OperatorType} {propertyValue}";

            var tickets = await _repository.QueryWithDtoAsync<TicketsAssignedToDto>(query);
            var assignedToIds = tickets.Select(y => y.AssignedTo);

            if (assignedToIds.Any())
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                var billUsersResponse = await _userManager.Users.AsNoTracking().Where(user => assignedToIds.Contains(user.Id) && user.IsDeleted == false).ToListAsync();
                var usersDetail = billUsersResponse.Adapt<List<UserDetailsDto>>();

                return await Result<List<UserDetailsDto>>.SuccessAsync(usersDetail);
            }
        }
        else if (usersBasedOnConditionsDto.Property == ConditionBasedOn.Orders)
        {
            string query = $@"SELECT ClientId FROM Orders GROUP BY ClientId HAVING COUNT(ClientId) {usersBasedOnConditionsDto.OperatorType} {propertyValue}";
            var orders = await _repository.QueryWithDtoAsync<Order>(query);
            var userIds = orders.Select(y => y.ClientId);

            if (userIds.Any())
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                var ordersUsersResponse = await _userManager.Users.AsNoTracking().Where(user => userIds.Contains(user.Id) && user.IsDeleted == false).ToListAsync();
                var usersDetail = ordersUsersResponse.Adapt<List<UserDetailsDto>>();

                return await Result<List<UserDetailsDto>>.SuccessAsync(usersDetail);
            }
        }
        else if (usersBasedOnConditionsDto.Property == ConditionBasedOn.Products)
        {
            string query = $@"SELECT AssignedToClientId FROM Products GROUP BY AssignedToClientId HAVING COUNT(AssignedToClientId) {usersBasedOnConditionsDto.OperatorType} {propertyValue}";
            var products = await _repository.QueryWithDtoAsync<Order>(query);
            var userIds = products.Select(y => y.ClientId);

            if (userIds.Any())
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                var response = await _userManager.Users.AsNoTracking().Where(user => userIds.Contains(user.Id) && user.IsDeleted == false).ToListAsync();
                var usersDetail = response.Adapt<List<UserDetailsDto>>();

                return await Result<List<UserDetailsDto>>.SuccessAsync(usersDetail);
            }
        }
        else if (usersBasedOnConditionsDto.Property == ConditionBasedOn.Refunds)
        {
            string query = $@"SELECT RequestById FROM Refunds GROUP BY RequestById HAVING COUNT(RequestById) {usersBasedOnConditionsDto.OperatorType} {propertyValue}";
            var items = await _repository.QueryWithDtoAsync<Order>(query);
            var userIds = items.Select(y => y.ClientId);

            if (userIds.Any())
            {
                List<ApplicationUser> users = new List<ApplicationUser>();
                var response = await _userManager.Users.AsNoTracking().Where(user => userIds.Contains(user.Id) && user.IsDeleted == false).ToListAsync();
                var usersDetail = response.Adapt<List<UserDetailsDto>>();

                return await Result<List<UserDetailsDto>>.SuccessAsync(usersDetail);
            }
        }

        return await Result<List<UserDetailsDto>>.SuccessAsync(new List<UserDetailsDto>());
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllUsersofAdminGroupAsync(string adminGroupId)
    {
        var users = await _userManager.Users.AsNoTracking()
            .Where(u => u.AdminGroupId == adminGroupId && u.IsDeleted == false).ToListAsync();
        var result = users.Adapt<List<UserDetailsDto>>();
        if (result != null)
        {
            foreach (var item in result)
            {
                var userRestrictedIps = await _repository.FindByConditionAsync<UserRestrictedIp>(m => m.UserId == item.Id.ToString());

                item.IpAddresses = userRestrictedIps?.Select(m => m.RestrictAccessIPAddress).ToList();

                var settings = await _repository.FindByConditionAsync<UserAppSetting>(x => x.UserId == item.Id.ToString());
                if (settings != null && settings.Any())
                {
                    var setting = settings.First();
                    item.AutoAssignOrders = setting.AutoAssignOrders;
                    item.CanTakeOrders = setting.CanTakeOrders;
                    item.AvaillableForOrders = setting.AvaillableForOrders;
                }

            }
        }

        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<IResult> CreateSubUserAsync(CreateSubUserRequest request, string origin)
    {
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            throw new IdentityException(string.Format(_localizer["User Email {0} is already taken."], request.Email));
        }

        if (string.IsNullOrEmpty(request.ParentID))
        {
            return await Result<string>.FailAsync(_localizer["Parent User Not Found."]);
        }

        var clientUser = await _userManager.Users.Where(u => u.Id == request.ParentID).FirstOrDefaultAsync();
        if (clientUser == null)
        {
            return await Result<string>.FailAsync(_localizer["Parent User Not Found."]);
        }

        if (string.IsNullOrEmpty(request.BrandId))
            request.BrandId = clientUser.BrandId;

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            CompanyName = request.CompanyName,
            Address1 = request.Address1,
            Address2 = request.Address2,
            City = request.City,
            State_Region = request.State_Region,
            ZipCode = request.ZipCode,
            Country = request.Country,
            PhoneNumber = request.PhoneNumber,
            Status = request.Status,
            IsActive = true,
            Tenant = _tenantService.GetCurrentTenant()?.Key,
            ParentID = request.ParentID,
            BrandId = request.BrandId,
            CreatedOn = DateTime.UtcNow

        };
        /*
        if (request.IpAddresses != null && request.IpAddresses.Count > 0)
        {
            foreach (string ipAddress in request.IpAddresses.Distinct())
            {
                var restrictIPId = await _repo.CreateAsync<UserRestrictedIp>(new UserRestrictedIp() { RestrictAccessIPAddress = ipAddress, UserId = user.Id });
                await _repo.SaveChangesAsync();
            }
        }
        */

        var result = await _userManager.CreateAsync(user, request.Password);
        if (result.Succeeded)
        {
            try
            {
                /*
                var spec = new BaseSpecification<UserModule>();
                var modules = await _repository.FindByConditionAsync<UserModule>(a => a.UserId == request.ParentID, true, spec);
                foreach (var item in modules.Where(m => m.Name == "Identity").ToList())
                {
                    var module = new UserModule(item.Name, item.PermissionDetail, item.Tenant, item.IsActive, user.Id);
                    module.DomainEvents.Add(new UserModuleCreatedEvent(module));
                    var moduleId = await _repository.CreateAsync<UserModule>((UserModule)module);
                    await _repository.SaveChangesAsync();
                }
                */

                foreach (var item in request.SubUserModules)
                {
                    var module = new UserModule(item.Name, item.PermissionDetail, item.Tenant, item.IsActive, user.Id);
                    module.DomainEvents.Add(new UserModuleCreatedEvent(module));
                    var moduleId = await _repository.CreateAsync<UserModule>((UserModule)module);
                    await _repository.SaveChangesAsync();
                }

                await _userManager.AddToRoleAsync(user, RoleConstants.Client);

                var errors = new List<string>();
                var roleNames = await _userManager.GetRolesAsync(user);
                foreach (var role in _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToList())
                {
                    var allClaims = await _roleManager.GetClaimsAsync(role);
                    if (!allClaims.Any(a => a.Type == ClaimConstants.Permission && a.Value == "Permission"))
                    {
                        var addResult = await _roleManager.AddPermissionClaimAsync(role, "Permission");
                        if (!addResult.Succeeded)
                        {
                            errors.AddRange(addResult.Errors.Select(e => _localizer[e.Description].ToString()));
                            return await Result<string>.FailAsync(errors);
                        }
                    }
                }
            }
            catch
            {
            }

            var messages = new List<string> { string.Format(_localizer["Sub User {0} Created."], user.UserName) };
            if (_mailSettings.EnableVerification)
            {
                // send verification email to subuser
                string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
                var userDto = user.Adapt<UserDetailsDto>();

                EmailTemplateDto emailDto = await _templateService.GenerateEmailConfirmationMail(userDto, emailVerificationUri);

                var mailRequest = new MailRequest
                {
                    From = _mailSettings.From,
                    To = new List<string> { user.Email },
                    Body = emailDto.Body,
                    Subject = emailDto.Subject
                };
                _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
                messages.Add(_localizer[$"Please check {user.Email} to verify your account!"]);
            }

            string message = $"Hello [[fullName]], a new sub user is created.";

            // send notification of change to parent user
            await SendMessageToParenUserAsync(request.ParentID, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.NEW_SUBUSER_CREATED, TargetUserTypes = NotificationTargetUserTypes.Clients, Data = new { SubUserID = user.Id } });

            return await Result<string>.SuccessAsync(user.Id, messages: messages);
        }
        else
        {
            throw new IdentityException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }
    }

    public async Task<IResult> UpdateSubUserAsync(CreateSubUserRequest request, string subUserId, string origin)
    {
        var user = await _userManager.FindByIdAsync(subUserId);
        if (user == null)
        {
            return await Result<string>.FailAsync(_localizer["Sub User Not Found."]);
        }

        user.UserName = request.Email;
        user.Email = request.Email;
        user.FullName = request.FullName;
        user.CompanyName = request.CompanyName;
        user.Address1 = request.Address1;
        user.Address2 = request.Address2;
        user.City = request.City;
        user.State_Region = request.State_Region;
        user.ZipCode = request.ZipCode;
        user.Country = request.Country;
        user.PhoneNumber = request.PhoneNumber;
        user.Status = request.Status;
        user.Tenant = _tenantService.GetCurrentTenant()?.Key;
        user.ParentID = request.ParentID;
        user.BrandId = request.BrandId;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            try
            {
                foreach (var item in request.SubUserModules)
                {
                    var spec = new BaseSpecification<UserModule>();
                    var modules = await _repository.FindByConditionAsync<UserModule>(a => a.UserId == item.SubUserId, true, spec);
                    if (modules == null || modules.Count() == 0) throw new EntityNotFoundException(string.Format(_localizer["modules.notfound"], item.Name));
                    var exisitngusermodule = modules.Where(m => m.Name == item.Name).FirstOrDefault();

                    if (exisitngusermodule == null)
                    {
                        bool moduleExists = await _repository.ExistsAsync<UserModule>(a => a.Name == item.Name && a.UserId == item.SubUserId && a.Tenant == item.Tenant);
                        if (moduleExists) throw new EntityAlreadyExistsException(string.Format(_localizer["module.alreadyexists"], item.Name));
                        var module = new UserModule(item.Name, item.PermissionDetail, item.Tenant, item.IsActive, item.SubUserId);
                        module.DomainEvents.Add(new UserModuleCreatedEvent(module));
                        var moduleId = await _repository.CreateAsync<UserModule>((UserModule)module);
                        await _repository.SaveChangesAsync();
                    }
                    else
                    {
                        var updatedModule = exisitngusermodule.Update(item.Name, item.PermissionDetail, item.IsActive, item.SubUserId);
                        updatedModule.DomainEvents.Add(new UserModuleDeletedEvent(updatedModule));
                        await _repository.UpdateAsync<UserModule>(updatedModule);
                        await _repository.SaveChangesAsync();
                    }
                }
            }
            catch
            {
            }

            var messages = new List<string> { string.Format(_localizer["User {0} Updated."], user.UserName) };
            if (_mailSettings.EnableVerification)
            {
                // send verification email to subuser
                string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
                var userDto = user.Adapt<UserDetailsDto>();

                EmailTemplateDto emailDto = await _templateService.GenerateEmailConfirmationMail(userDto, emailVerificationUri);

                var mailRequest = new MailRequest
                {
                    From = _mailSettings.From,
                    To = new List<string> { user.Email },
                    Body = emailDto.Body,
                    Subject = emailDto.Subject
                };
                _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
                messages.Add(_localizer[$"Please check {user.Email} to verify your account!"]);
            }

            string message = $"Hello [[fullName]], a new sub user is updated.";

            // send notification of change to parent user
            await SendMessageToParenUserAsync(request.ParentID, new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.NEW_SUBUSER_CREATED, TargetUserTypes = NotificationTargetUserTypes.Clients, Data = new { SubUserID = user.Id } });

            return await Result<string>.SuccessAsync(user.Id, messages: messages);
        }
        else
        {
            throw new IdentityException(_localizer["Validation Errors Occurred."], result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }
    }

    public async Task MailAdminUser(string messege, string subject)
    {
        var mailRequest = new MailRequest
        {
            From = _mailSettings.From,
            To = new List<string> { _user.GetUserEmail() },
            Body = messege,
            Subject = subject
        };
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
        await Task.CompletedTask;
    }

    private async Task SendMessageToParenUserAsync(string userId, INotificationMessage notification, bool invokeSaveChanges = true)
    {
        DateTime sentAt = DateTime.UtcNow;
        await _notificationHubContext.Clients.User(userId).SendAsync(notification.MessageType, notification);

        var notificationToAdd = new Notification(Guid.Parse(userId), notification.NotificationType, NotificationStatus.Sent, notification.TargetUserTypes, sentAt, notification.Message, notification.Title, null, notification.NotifyModelId);
        await _repository.CreateAsync<Notification>(notificationToAdd);

        if (invokeSaveChanges)
            await _repository.SaveChangesAsync();
    }

    private async Task<string> GetEmailVerificationUriAsync(ApplicationUser user, string origin)
    {
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        origin = await GetDomainApiUriAsync(user, origin);

        const string route = "verify-email";
        var endpointUri = new Uri(string.Concat($"{origin}/", $"{route}/", $"{user.Id}"));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "code", code);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, "tenant", _tenantService.GetCurrentTenant()?.Key);

        if (!string.IsNullOrEmpty(user.BrandId))
        {
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "brandId", user.BrandId);
        }

        return verificationUri;
    }

    private async Task<string> GetDomainApiUriAsync(ApplicationUser user, string origin)
    {
        if (await _userManager.IsInRoleAsync(user, RoleConstants.Admin))
        {
            origin = string.Concat($"{origin}/", "admin");
        }
        else if (await _userManager.IsInRoleAsync(user, RoleConstants.Client))
        {
            origin = string.Concat($"{origin}/", "client");
        }

        return origin;
    }

}