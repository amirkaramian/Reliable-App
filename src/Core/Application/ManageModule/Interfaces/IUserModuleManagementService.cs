using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Application.ManageModule.Interfaces;

public interface IUserModuleManagementService : ITransientService
{
    Task<PaginatedResult<UserModuleDto>> SearchAsync(UserModuleManagementListFilter filter);

    Task<Result<Guid>> CreateUserModuleManagementAsync(CreateUserModuleManagementRequest request);

    Task<Result<Guid>> UpdateUserModuleManagementAsync(UpdateUserModuleManagementRequest request, Guid id);

    Task<Result<Guid>> UpdateSubUserModuleManagementAsync(UpdateUserModuleManagementRequest request);

    Task<Result<Guid>> UpdateSubUserModuleListAsync(UpdateSubUserModuleListRequest request);

    Task<Result<Guid>> DeleteUserModuleManagementAsync(Guid id);
    Task<Result<UserModuleDto>> GetUserModuleManagementAsync(Guid id);
    Task<Result<List<UserModuleDto>>> GetUserModuleManagementByUserIdAsync(string userId);
}