using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Departments;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Departments.Interfaces;

public interface IDepartmentService : ITransientService
{
    Task<PaginatedResult<DepartmentDto>> SearchAsync(DepartmentListFilter filter);

    Task<Result<Guid>> CreateDepartmentAsync(CreateDepartmentRequest request);
    Task<Result<Guid>> AssignDepartmentAsync(AssignDepartmentRequest request);
    Task<Result<Guid>> UnAssignDepartmentAsync(AssignDepartmentRequest request);
    Task<Result<Guid>> UpdateDepartmentAsync(UpdateDepartmentRequest request, Guid id);

    Task<Result<Guid>> DeleteDepartmentAsync(Guid id);
    Task<Result<DepartmentDto>> GetDepartmentAsync(Guid id);
    Task<Result<List<UserDetailsDto>>> GetDepartmentUsersAsync(Guid id);
    Task<Result<DepartmentDto>> GetDepartmentAsync(string deptName);
    Task<Result<List<DepartmentAdminAssignStatusDto>>> GetDepartmentByUserIdAsync(Guid userid);
    Task<Result<List<DepartmentDto>>> GetDepartmentByTenantAsync(string tenant);
}
