using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Departments.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Departments;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Application.Specifications;
using Mapster;
using MyReliableSite.Domain.Departments.Events;
using MyReliableSite.Shared.DTOs.Notifications;
using MyReliableSite.Shared.DTOs.Notifications.Enums;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Departments.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IStringLocalizer<DepartmentService> _localizer;
    private readonly IRepositoryAsync _repository;
    private readonly ICurrentUser _currentUser;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;
    public DepartmentService()
    {
    }

    public DepartmentService(IRepositoryAsync repository, IUserService userService, ICurrentUser currentUser, IStringLocalizer<DepartmentService> localizer, INotificationService notificationService)
    {
        _repository = repository;
        _currentUser = currentUser;
        _localizer = localizer;
        _notificationService = notificationService;
        _userService = userService;
    }

    public async Task<Result<Guid>> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        bool departmentExists = await _repository.ExistsAsync<Department>(a => a.Name == request.Name);
        if (departmentExists) throw new EntityAlreadyExistsException(string.Format(_localizer["Department.alreadyexists"], request.Name));
        var department = new Department(request.Name, request.DeptNumber, request.DeptStatus, request.IsDefault, request.BrandId);
        department.DomainEvents.Add(new DepartmentCreatedEvent(department));
        var departmentId = await _repository.CreateAsync<Department>((Department)department);
        await _repository.SaveChangesAsync();

        if (department.DepartmentAdmins == null)
        {
            department.DepartmentAdmins = new List<DepartmentAdmin>();
        }

        foreach (var adminId in request.DepartmentAdmins)
        {
            department.DepartmentAdmins.Add(new DepartmentAdmin()
            {
                AdminUserId = adminId,
                Department = department,
                DepartmentId = departmentId
            });

            string message = $"Hello [[fullName]], you're added into Department group {request.Name}";

            await _notificationService.SendMessageToUserAsync(Convert.ToString(adminId), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ADDED_TO_DEPARTMENT_GROUP, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = departmentId, Data = new { DepartmentId = departmentId } }, false);
        }

        if (department.DepartmentAdmins != null && department.DepartmentAdmins.Count > 0)
        {
            _ = await _repository.CreateRangeAsync<DepartmentAdmin>(department.DepartmentAdmins);
            _ = await _repository.SaveChangesAsync();
        }

        return await Result<Guid>.SuccessAsync(departmentId);
    }

    public async Task<Result<Guid>> AssignDepartmentAsync(AssignDepartmentRequest request)
    {
        var spec = new BaseSpecification<Department>();
        spec.Includes.Add(m => m.DepartmentAdmins);
        var department = await _repository.GetByIdAsync<Department>(request.DepartmentId, spec);
        if (department == null) throw new EntityNotFoundException(string.Format(_localizer["Department.notfound"], request.DepartmentId));
        if (department.DepartmentAdmins == null)
        {
            department.DepartmentAdmins = new List<DepartmentAdmin>();
        }

        if (department.DepartmentAdmins.Count(m => m.AdminUserId == request.UserId) > 0)
        {
            throw new EntityAlreadyExistsException(string.Format(_localizer["Department.useralreadyassigned"], request.DepartmentId));
        }

        var departmentAssign = new DepartmentAdmin()
        {
            AdminUserId = request.UserId,
            Department = department,
            DepartmentId = request.DepartmentId
        };

        string message = $"Hello [[fullName]], you're added into Department group {_currentUser.Name}";

        await _notificationService.SendMessageToUserAsync(Convert.ToString(request.UserId), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ADDED_TO_DEPARTMENT_GROUP, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = request.DepartmentId, Data = new { DepartmentId = request.DepartmentId } }, false);

        var departmentId = await _repository.CreateAsync<DepartmentAdmin>(departmentAssign);
        _ = await _repository.SaveChangesAsync();

        await _repository.ClearCacheAsync<Department>(department);
        return await Result<Guid>.SuccessAsync(departmentId);

    }

    public async Task<Result<Guid>> UnAssignDepartmentAsync(AssignDepartmentRequest request)
    {
        var spec = new BaseSpecification<Department>();
        spec.Includes.Add(m => m.DepartmentAdmins);
        var department = await _repository.GetByIdAsync<Department>(request.DepartmentId, spec);
        if (department == null) throw new EntityNotFoundException(string.Format(_localizer["Department.notfound"], request.DepartmentId));
        if (department.DepartmentAdmins == null)
        {
            department.DepartmentAdmins = new List<DepartmentAdmin>();
        }

        if (department.DepartmentAdmins.Count(m => m.DepartmentId == request.DepartmentId) == 0)
        {
            throw new EntityNotFoundException(string.Format(_localizer["Department.alreadynotassigned"], request.DepartmentId));
        }

        var departmentAdmin = await _repository.FirstByConditionAsync<DepartmentAdmin>(m => m.DepartmentId == request.DepartmentId && m.AdminUserId == request.UserId);

        var departmentToDelete = await _repository.RemoveByIdAsync<DepartmentAdmin>(departmentAdmin.Id);

        await _repository.SaveChangesAsync();

        await _repository.ClearCacheAsync<Department>(department);

        return await Result<Guid>.SuccessAsync(departmentAdmin.Id);

    }

    public async Task<Result<Guid>> DeleteDepartmentAsync(Guid id)
    {
        var department = await _repository.GetByIdAsync<Department>(id);
        if (department == null) throw new EntityNotFoundException(string.Format(_localizer["Department.notfound"], id));
        if (department.IsDefault)
        {
            throw new InvalidOperationException(string.Format(_localizer["This department is default, can't be deleted"]));
        }

        // user assignment to default department
        var departmentDefault = await _repository.FirstByConditionAsync<Department>(m => m.IsDefault);

        if (departmentDefault != null)
        {
            var departmentAdmins = await _repository.GetListAsync<DepartmentAdmin>(m => m.DepartmentId == id);
            if (departmentDefault != null && departmentDefault.DepartmentAdmins != null && departmentAdmins != null && departmentAdmins.Any() && departmentDefault.DepartmentAdmins.Any())
            {
                departmentAdmins = departmentAdmins.Where(m => !departmentDefault.DepartmentAdmins.Select(x => x.AdminUserId).Contains(m.AdminUserId)).ToList();
            }

            if (departmentDefault.DepartmentAdmins == null)
            {
                departmentDefault.DepartmentAdmins = new List<DepartmentAdmin>();
            }

            foreach (var adminId in departmentAdmins)
            {
                departmentDefault.DepartmentAdmins.Add(new DepartmentAdmin()
                {
                    AdminUserId = adminId.AdminUserId,
                    Department = departmentDefault,
                    DepartmentId = id
                });

                string message = $"Hello [[fullName]], you're added into Department group {departmentDefault?.Name}";

                await _notificationService.SendMessageToUserAsync(Convert.ToString(adminId), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ADDED_TO_DEPARTMENT_GROUP, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = id, Data = new { DepartmentId = id } }, false);
            }
        }

        var departmentToDelete = await _repository.RemoveByIdAsync<Department>(id);
        departmentToDelete.DomainEvents.Add(new DepartmentDeletedEvent(departmentToDelete));

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<DepartmentDto>> SearchAsync(DepartmentListFilter filter)
    {
        var departments = await _repository.GetSearchResultsAsync<Department, DepartmentDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);

        foreach (var item in departments.Data)
        {
            var departmentAdmins = await _repository.GetListAsync<DepartmentAdmin>(m => m.DepartmentId == item.Id);
            var ids = departmentAdmins.Select(m => m.AdminUserId);
            item.DepartmentAdminsList = ids.ToList<Guid>();
        }

        return departments;
    }

    public async Task<Result<Guid>> UpdateDepartmentAsync(UpdateDepartmentRequest request, Guid id)
    {
        var spec = new BaseSpecification<Department>();
        spec.Includes.Add(m => m.DepartmentAdmins);
        var department = await _repository.GetByIdAsync<Department>(id, spec);
        if (department == null) throw new EntityNotFoundException(string.Format(_localizer["Department.notfound"], id));
        var updatedDepartment = department.Update(request.Name, request.DeptNumber, request.DeptStatus, request.IsDefault, request.BrandId);

        if (updatedDepartment.DepartmentAdmins != null && updatedDepartment.DepartmentAdmins.Count > 0)
        {
            await _repository.ClearAsync<DepartmentAdmin>(m => m.DepartmentId == id);
        }

        if (updatedDepartment.DepartmentAdmins == null)
        {
            updatedDepartment.DepartmentAdmins = new List<DepartmentAdmin>();
        }
        else
        {
            updatedDepartment.DepartmentAdmins.Clear();
        }

        foreach (var adminId in request.DepartmentAdmins)
        {
            updatedDepartment.DepartmentAdmins.Add(new DepartmentAdmin()
            {
                AdminUserId = adminId,
                Department = updatedDepartment,
                DepartmentId = id
            });

            string message = $"Hello [[fullName]], you're added into Department group {request.Name}";
            await _notificationService.SendMessageToUserAsync(Convert.ToString(adminId), new BasicNotification() { Message = message, Label = BasicNotification.LabelType.Information, NotificationType = NotificationType.ADDED_TO_DEPARTMENT_GROUP, TargetUserTypes = NotificationTargetUserTypes.Admins, NotifyModelId = id, Data = new { DepartmentId = id } }, false);
        }

        if (updatedDepartment.DepartmentAdmins != null && updatedDepartment.DepartmentAdmins.Count > 0)
        {
            _ = await _repository.CreateRangeAsync<DepartmentAdmin>(updatedDepartment.DepartmentAdmins);
        }

        updatedDepartment.DomainEvents.Add(new DepartmentUpdatedEvent(updatedDepartment));
        await _repository.UpdateAsync<Department>(updatedDepartment);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<DepartmentDto>> GetDepartmentAsync(Guid id)
    {
        var spec = new BaseSpecification<Department>();
        spec.Includes.Add(m => m.DepartmentAdmins);
        spec.Includes.Add(o => o.Brand);

        var department = await _repository.GetByIdAsync<Department, DepartmentDto>(id, spec);
        return await Result<DepartmentDto>.SuccessAsync(department);
    }

    public async Task<Result<List<UserDetailsDto>>> GetDepartmentUsersAsync(Guid id)
    {
        var departmentAdmins = await _repository.GetListAsync<DepartmentAdmin>(m => m.DepartmentId == id);

        return await _userService.GetAllAsync(departmentAdmins.Select(m => m.AdminUserId.ToString()).ToList());
    }

    public async Task<Result<List<DepartmentAdminAssignStatusDto>>> GetDepartmentByUserIdAsync(Guid userid)
    {
        var departments = await _repository.FindByConditionAsync<Department>(m => m.Tenant == _currentUser.GetTenant());
        var spec = new BaseSpecification<Department>();
        spec.Includes.Add(m => m.DepartmentAdmins);
        spec.Includes.Add(o => o.Brand);

        var departListIds = await _repository.GetListAsync<DepartmentAdmin>(m => m.AdminUserId == userid);
        List<DepartmentAdminAssignStatusDto> departmentAdminAssignStatuses = new List<DepartmentAdminAssignStatusDto>();
        foreach (var item in departments)
        {
            departmentAdminAssignStatuses.Add(new DepartmentAdminAssignStatusDto() { DepartmentName = item.Name, DepartmentId = item.Id, isAssign = departListIds.Exists(m => m.DepartmentId == item.Id) });
        }

        return await Result<List<DepartmentAdminAssignStatusDto>>.SuccessAsync(departmentAdminAssignStatuses);
    }

    public async Task<Result<DepartmentDto>> GetDepartmentAsync(string deptName)
    {
        var spec = new BaseSpecification<Department>();
        spec.Includes.Add(m => m.DepartmentAdmins);
        spec.Includes.Add(m => m.Brand);

        var department = await _repository.FirstByConditionAsync<Department>(m => m.Name.ToLower().Equals(deptName.ToLower()), false, spec);
        var mappedArticle = department.Adapt<DepartmentDto>();
        return await Result<DepartmentDto>.SuccessAsync(mappedArticle);
    }

    public async Task<Result<List<DepartmentDto>>> GetDepartmentByTenantAsync(string tenant)
    {
        var spec = new BaseSpecification<Department>();
        spec.Includes.Add(m => m.DepartmentAdmins);
        spec.Includes.Add(m => m.Brand);

        var departments = await _repository.FindByConditionAsync<Department>(m => m.Tenant == tenant, false, spec);
        return await Result<List<DepartmentDto>>.SuccessAsync(departments.Adapt<List<DepartmentDto>>());
    }

}
