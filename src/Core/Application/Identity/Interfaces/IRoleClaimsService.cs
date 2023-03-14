using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Abstractions.Services.Identity;

public interface IRoleClaimsService : ITransientService
{

    public Task<bool> RoleHasPermissionAsync(List<string> apiRoles, string apiKey, string permission);

    public Task<bool> HasPermissionAsync(string userId, string permission);

    Task<Result<List<RoleClaimResponse>>> GetAllAsync();

    Task<int> GetCountAsync();

    Task<Result<RoleClaimResponse>> GetByIdAsync(int id);

    Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId);

    Task<Result<string>> SaveAsync(RoleClaimRequest request);

    Task<Result<string>> DeleteAsync(int id);

    Task<List<string>> GetAllUsersHasPermissions(string permission);

}
