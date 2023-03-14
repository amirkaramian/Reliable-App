using Microsoft.Extensions.Localization;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.ManageModule;
using MyReliableSite.Application.Specifications;
using Mapster;

namespace MyReliableSite.Application.ManageModule.Services;

public class AdminGroupModuleManagementService : IAdminGroupModuleManagementService
{
    private readonly IStringLocalizer<AdminGroupModuleManagementService> _localizer;
    private readonly IRepositoryAsync _repository;

    public AdminGroupModuleManagementService()
    {
    }

    public AdminGroupModuleManagementService(IRepositoryAsync repository, IStringLocalizer<AdminGroupModuleManagementService> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<Result<Guid>> CreateAdminGroupModuleManagementAsync(CreateAdminGroupModuleManagementRequest request)
    {
        bool moduleExists = await _repository.ExistsAsync<AdminGroupModule>(a => a.Name == request.Name && a.AdminGroupId == request.AdminGroupId && a.Tenant == request.Tenant);
        if (moduleExists) throw new EntityAlreadyExistsException(string.Format(_localizer["module.alreadyexists"], request.Name));
        var module = new AdminGroupModule(request.Name, request.PermissionDetail, request.Tenant, request.IsActive, request.AdminGroupId);
        module.DomainEvents.Add(new StatsChangedEvent());
        var moduleId = await _repository.CreateAsync<AdminGroupModule>((AdminGroupModule)module);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(moduleId);
    }

    public async Task<Result<Guid>> DeleteAdminGroupModuleManagementAsync(Guid id)
    {
        var moduleToDelete = await _repository.RemoveByIdAsync<AdminGroupModule>(id);
        moduleToDelete.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> DeleteAdminGroupByAdminGroupIdModuleManagementAsync(string adminGroupId)
    {
        await _repository.ClearAsync<AdminGroupModule>(m => m.AdminGroupId == adminGroupId);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(adminGroupId);
    }

    public async Task<PaginatedResult<AdminGroupModuleDto>> SearchAsync(AdminGroupModuleManagementListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<AdminGroupModule, AdminGroupModuleDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<Guid>> UpdateAdminGroupModuleManagementAsync(UpdateAdminGroupModuleManagementRequest request, Guid id)
    {
        var module = await _repository.GetByIdAsync<AdminGroupModule>(id);
        if (module == null) throw new EntityNotFoundException(string.Format(_localizer["module.notfound"], id));
        var updatedModule = module.Update(request.Name, request.PermissionDetail, request.IsActive, request.AdminGroupId);
        updatedModule.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<AdminGroupModule>(updatedModule);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<AdminGroupModuleDto>> GetAdminGroupModuleManagementAsync(Guid id)
    {
        var spec = new BaseSpecification<AdminGroupModule>();
        var module = await _repository.GetByIdAsync<AdminGroupModule, AdminGroupModuleDto>(id, spec);
        return await Result<AdminGroupModuleDto>.SuccessAsync(module);
    }

    public async Task<Result<List<AdminGroupModuleDto>>> GetAdminGroupModuleManagementByAdminGroupIdAsync(string adminGroupId)
    {
        var spec = new BaseSpecification<AdminGroupModule>();
        var modules = await _repository.FindByConditionAsync<AdminGroupModule>(a => a.AdminGroupId == adminGroupId, true, spec);
        var dto = modules.Adapt<List<AdminGroupModuleDto>>();
        return await Result<List<AdminGroupModuleDto>>.SuccessAsync(dto);
    }
}