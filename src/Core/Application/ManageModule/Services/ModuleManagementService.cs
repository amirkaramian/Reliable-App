using Microsoft.Extensions.Localization;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.ManageModule;
using MyReliableSite.Application.Specifications;
using Newtonsoft.Json.Linq;
using MyReliableSite.Application.Settings;
using Newtonsoft.Json;
using Mapster;

namespace MyReliableSite.Application.ManageModule.Services;

public class ModuleManagementService : IModuleManagementService
{
    private readonly IStringLocalizer<ModuleManagementService> _localizer;
    private readonly IRepositoryAsync _repository;

    public ModuleManagementService()
    {
    }

    public ModuleManagementService(IRepositoryAsync repository, IStringLocalizer<ModuleManagementService> localizer)
    {
        _repository = repository;
        _localizer = localizer;
    }

    public async Task<Result<Guid>> CreateModuleManagementAsync(CreateModuleManagementRequest request, string filePath)
    {
        bool moduleExists = await _repository.ExistsAsync<Module>(a => a.Name == request.Name && a.Tenant == request.Tenant);
        if (moduleExists) throw new EntityAlreadyExistsException(string.Format(_localizer["module.alreadyexists"], request.Name));
        var module = new Module(request.Name, request.PermissionDetail, request.Tenant, request.IsActive);
        module.DomainEvents.Add(new StatsChangedEvent());
        var moduleId = await _repository.CreateAsync<Module>((Module)module);
        await _repository.SaveChangesAsync();

        // adding module to json file
        AddModule(filePath, ReadModuleJson(filePath), request);

        return await Result<Guid>.SuccessAsync(moduleId);
    }

    public async Task<Result<Guid>> DeleteModuleManagementAsync(Guid id, string filePath)
    {
        var moduleToDelete = await _repository.RemoveByIdAsync<Module>(id);
        moduleToDelete.DomainEvents.Add(new StatsChangedEvent());

        // adding module to json file
        DeleteModule(filePath, ReadModuleJson(filePath), moduleToDelete.Name, moduleToDelete.Tenant);

        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<PaginatedResult<ModuleDto>> SearchAsync(ModuleManagementListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<Module, ModuleDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<Guid>> UpdateModuleManagementAsync(UpdateModuleManagementRequest request, Guid id, string filePath)
    {
        var module = await _repository.GetByIdAsync<Module>(id);
        if (module == null) throw new EntityNotFoundException(string.Format(_localizer["module.notfound"], id));
        var updatedModule = module.Update(request.Name, request.PermissionDetail, request.IsActive);
        updatedModule.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync<Module>(updatedModule);
        await _repository.SaveChangesAsync();

        // adding module to json file
        UpdateModule(filePath, ReadModuleJson(filePath), request);

        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<ModuleDto>> GetModuleManagementAsync(Guid id)
    {
        var spec = new BaseSpecification<Module>();
        var module = await _repository.GetByIdAsync<Module, ModuleDto>(id, spec);
        return await Result<ModuleDto>.SuccessAsync(module);
    }

    public async Task<Result<List<ModuleDto>>> GetModuleManagementByTenantIdAsync(string Tenant)
    {
        var spec = new BaseSpecification<Module>();
        var modules = await _repository.FindByConditionAsync<Module>(a => a.Tenant == Tenant, true, spec);
        var dto = modules.Adapt<List<ModuleDto>>();
        return await Result<List<ModuleDto>>.SuccessAsync(dto);
    }

    private void AddModule(string fileName, ModuleManagementSettings moduleManagementSettings, CreateModuleManagementRequest request)
    {
        foreach (var item in moduleManagementSettings.ModuleManagements)
        {
            if (item.Tenant == request.Tenant)
            {
                item.Modules.Add(new ModuleAppLevel()
                {
                    Name = request.Name,
                    Permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(request.PermissionDetail),
                    IsActive = request.IsActive
                });
                break;
            }
        }

        WriteModuleJson(fileName, moduleManagementSettings);
    }

    private void UpdateModule(string fileName, ModuleManagementSettings moduleManagementSettings, UpdateModuleManagementRequest request)
    {
        foreach (var item in moduleManagementSettings.ModuleManagements)
        {
            if (item.Tenant == request.Tenant)
            {
                bool found = false;
                foreach (var module in item.Modules)
                {
                    if (module.Name == request.Name)
                    {
                        module.Name = request.Name;
                        module.Permissions = JsonConvert.DeserializeObject<Dictionary<string, bool>>(request.PermissionDetail);
                        module.IsActive = request.IsActive;
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    break;
                }
            }
        }

        WriteModuleJson(fileName, moduleManagementSettings);
    }

    private void DeleteModule(string fileName, ModuleManagementSettings moduleManagementSettings, string moduleName, string Tenant)
    {
        foreach (var item in moduleManagementSettings.ModuleManagements)
        {
            if (item.Tenant == Tenant)
            {
                var toBeDelete = item.Modules.First(m => m.Name == moduleName);
                if (toBeDelete != null)
                    item.Modules.Remove(toBeDelete);
                break;
            }
        }

        WriteModuleJson(fileName, moduleManagementSettings);
    }

    private ModuleManagementSettings ReadModuleJson(string fileName)
    {
        string jsonString = File.ReadAllText(fileName);
        return JsonConvert.DeserializeObject<ModuleManagementSettings>(jsonString);
    }

    private void WriteModuleJson(string fileName, ModuleManagementSettings moduleManagementSettings)
    {
        string jsonString = JsonConvert.SerializeObject(moduleManagementSettings);
        File.WriteAllText(fileName, jsonString);
    }
}
