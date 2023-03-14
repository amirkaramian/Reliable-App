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
using MyReliableSite.Domain.ManageModule.Events;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Application.ManageModule.Services;

public class UserModuleManagementService : IUserModuleManagementService
{
    private readonly IStringLocalizer<UserModuleManagementService> _localizer;
    private readonly IRepositoryAsync _repository;

    public UserModuleManagementService()
    {
    }

    public UserModuleManagementService(IRepositoryAsync repository, IStringLocalizer<UserModuleManagementService> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<Result<Guid>> CreateUserModuleManagementAsync(CreateUserModuleManagementRequest request)
    {
        bool moduleExists = await _repository.ExistsAsync<UserModule>(a => a.Name == request.Name && a.UserId == request.UserId && a.Tenant == request.Tenant);
        if (moduleExists) throw new EntityAlreadyExistsException(string.Format(_localizer["module.alreadyexists"], request.Name));
        var module = new UserModule(request.Name, request.PermissionDetail, request.Tenant, request.IsActive, request.UserId);
        module.DomainEvents.Add(new UserModuleCreatedEvent(module));
        var moduleId = await _repository.CreateAsync<UserModule>((UserModule)module);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(moduleId);
    }

    public async Task<Result<Guid>> DeleteUserModuleManagementAsync(Guid id)
    {
        var moduleToDelete = await _repository.RemoveByIdAsync<UserModule>(id);
        moduleToDelete.DomainEvents.Add(new UserModuleDeletedEvent(moduleToDelete));

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<UserModuleDto>> SearchAsync(UserModuleManagementListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<UserModule, UserModuleDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<Guid>> UpdateUserModuleManagementAsync(UpdateUserModuleManagementRequest request, Guid id)
    {
        var module = await _repository.GetByIdAsync<UserModule>(id);
        if (module == null) throw new EntityNotFoundException(string.Format(_localizer["module.notfound"], id));
        var updatedModule = module.Update(request.Name, request.PermissionDetail, request.IsActive, request.UserId);
        updatedModule.DomainEvents.Add(new UserModuleDeletedEvent(updatedModule));
        await _repository.UpdateAsync<UserModule>(updatedModule);
        await _repository.SaveChangesAsync();

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateSubUserModuleManagementAsync(UpdateUserModuleManagementRequest request)
    {
        var spec = new BaseSpecification<UserModule>();
        var exisitingsubusermodules = await _repository.FindByConditionAsync<UserModule>(a => a.UserId == request.UserId, true, spec);
        if (exisitingsubusermodules == null || exisitingsubusermodules.Count() == 0) throw new EntityNotFoundException(string.Format(_localizer["module.notfound"], request.Name));
        var exisitingsubusermodule = exisitingsubusermodules.Where(m => m.Name == request.Name).FirstOrDefault();
        if (exisitingsubusermodule == null)
        {
            var module = new UserModule(request.Name, request.PermissionDetail, request.Tenant, request.IsActive, request.UserId);
            module.DomainEvents.Add(new UserModuleCreatedEvent(module));
            var moduleId = await _repository.CreateAsync<UserModule>((UserModule)module);
            await _repository.SaveChangesAsync();
        }
        else
        {
            var updatedModule = exisitingsubusermodule.Update(request.Name, request.PermissionDetail, request.IsActive, request.UserId);
            updatedModule.DomainEvents.Add(new UserModuleDeletedEvent(updatedModule));
            await _repository.UpdateAsync<UserModule>(updatedModule);
            await _repository.SaveChangesAsync();
        }

        return await Result<Guid>.SuccessAsync(request.UserId);
    }

    public async Task<Result<Guid>> UpdateSubUserModuleListAsync(UpdateSubUserModuleListRequest request)
    {
        string subUserID = Guid.Empty.ToString();

        foreach (var item in request.SubUserModules)
        {
            subUserID = item.SubUserId;
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

        return await Result<Guid>.SuccessAsync(subUserID);
    }

    public async Task<Result<UserModuleDto>> GetUserModuleManagementAsync(Guid id)
    {
        var spec = new BaseSpecification<UserModule>();
        var module = await _repository.GetByIdAsync<UserModule, UserModuleDto>(id, spec);
        return await Result<UserModuleDto>.SuccessAsync(module);
    }

    public async Task<Result<List<UserModuleDto>>> GetUserModuleManagementByUserIdAsync(string userId)
    {
        var spec = new BaseSpecification<UserModule>();
        var modules = await _repository.FindByConditionAsync<UserModule>(a => a.UserId == userId, true, spec);
        var dto = modules.Adapt<List<UserModuleDto>>();
        return await Result<List<UserModuleDto>>.SuccessAsync(dto);
    }
}