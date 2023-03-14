using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Application.ManageModule.Interfaces;

public interface IModuleManagementService : ITransientService
{
    Task<PaginatedResult<ModuleDto>> SearchAsync(ModuleManagementListFilter filter);

    Task<Result<Guid>> CreateModuleManagementAsync(CreateModuleManagementRequest request, string filePath);

    Task<Result<Guid>> UpdateModuleManagementAsync(UpdateModuleManagementRequest request, Guid id, string filePath);

    Task<Result<Guid>> DeleteModuleManagementAsync(Guid id, string filePath);
    Task<Result<ModuleDto>> GetModuleManagementAsync(Guid id);
    Task<Result<List<ModuleDto>>> GetModuleManagementByTenantIdAsync(string Tenant);
}
