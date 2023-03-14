using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Identity.Interfaces;

public interface IUserLoginHistoryService : ITransientService
{
    Task<Result<Guid>> CreateUserLoginHistoryAsync(CreateUserLoginHistoryRequest request);
    Task<PaginatedResult<UserLoginHistoryDto>> SearchAsync(UserLoginHistoryListFilter filter);
    Task<Result<UserLoginHistoryDto>> GetUserLoginHistoryAsync(Guid id);
    Task<Result<List<UserLoginHistoryDto>>> GetUserLoginHistoryByUserIdAsync(string userId);
}
