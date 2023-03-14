using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Identity.Interfaces;

public interface IAdminGroupService : ITransientService
{
    Task<Result<Guid>> CreateAdminGroupAsync(CreateAdminGroupRequest request);
    Task<Result<Guid>> DeleteAdminGroupAsync(Guid id);
    Task<PaginatedResult<AdminGroupDto>> SearchAsync(AdminGroupListFilter filter);
    Task<Result<Guid>> UpdateAdminGroupAsync(UpdateAdminGroupRequest request, Guid id);
    Task<Result<AdminGroupDto>> GetAdminGroupAsync(Guid id);
    Task<Result<AdminGroupDto>> GetDefaultAdminGroupAsync();
    Task<Result<AdminGroupDto>> GetSuperAdminGroupAsync();
}
