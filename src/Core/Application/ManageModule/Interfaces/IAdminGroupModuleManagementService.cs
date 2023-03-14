using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Application.ManageModule.Interfaces;

public interface IAdminGroupModuleManagementService : ITransientService
{
    Task<PaginatedResult<AdminGroupModuleDto>> SearchAsync(AdminGroupModuleManagementListFilter filter);

    Task<Result<Guid>> CreateAdminGroupModuleManagementAsync(CreateAdminGroupModuleManagementRequest request);

    Task<Result<Guid>> UpdateAdminGroupModuleManagementAsync(UpdateAdminGroupModuleManagementRequest request, Guid id);

    Task<Result<Guid>> DeleteAdminGroupModuleManagementAsync(Guid id);
    Task<Result<Guid>> DeleteAdminGroupByAdminGroupIdModuleManagementAsync(string adminGroupId);
    Task<Result<AdminGroupModuleDto>> GetAdminGroupModuleManagementAsync(Guid id);
    Task<Result<List<AdminGroupModuleDto>>> GetAdminGroupModuleManagementByAdminGroupIdAsync(string adminGroupId);
}