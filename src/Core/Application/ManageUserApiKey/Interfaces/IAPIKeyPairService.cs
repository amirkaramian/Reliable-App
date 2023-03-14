using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.ManageUserApiKey;
using MyReliableSite.Shared.DTOs.ManageSubUserApiKey;
using MyReliableSite.Shared.DTOs.ManageUserApiKey;

namespace MyReliableSite.Application.ManageUserApiKey.Interfaces;

public interface IAPIKeyPairService : ITransientService
{
    Task<PaginatedResult<APIKeyPairDto>> SearchAsync(APIKeyPairListFilter filter);

    Task<Result<IEnumerable<APIKeyPairDto>>> GetAllAsync();

    Task<Result<Guid>> CreateAPIKeyPairAsync(CreateAPIKeyPairRequest request);
    Task<Result<Guid>> CreateSubUserAPIKeyPairAsync(CreateSubUserAPIKeyPairRequest request);

    Task<Result<Guid>> UpdateAPIKeyPairAsync(UpdateAPIKeyPairRequest request, Guid id);

    Task<Result<Guid>> UpdateSubUserAPIKeyPairAsync(UpdateSubUserAPIKeyPairRequest request, Guid id);

    Task<Result<Guid>> UpdateAPIKeyPairPermissionsAsync(UpdateAPIKeyPairPermissionRequest request, Guid id);
    Task<Result<Guid>> UpdateSubUserAPIKeyPairPermissionsAsync(UpdateSubUserAPIKeyPairPermissionRequest request, Guid id);

    Task<Result<Guid>> DeleteAPIKeyPairAsync(Guid id);
    Task<Result<APIKeyPairDto>> GetAPIKeyPairAsync(Guid id);
    Task<Result<APIKeyPairDto>> GetAPIKeyPairAsync(string appName);

    Task<Result<List<APIKeyPairEXL>>> GetAPIKeyPairListAsync(string userId, DateTime startDate, DateTime endDate);
}
