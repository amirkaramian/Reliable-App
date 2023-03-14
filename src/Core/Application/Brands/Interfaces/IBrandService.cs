using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Application.Brands.Interfaces;

public interface IBrandService : ITransientService
{
    Task<Result<BrandDto>> GetAsync(Guid id);

    Task<Result<List<UserDetailsDto>>> GetBrandUsersAsync(Guid id);
    Task<PaginatedResult<BrandDto>> SearchAsync(BrandListFilter filters);

    Task<Result<BrandDto>> CreateAsync(CreateBrandRequest request);

    Task<Result<BrandDto>> UpdateAsync(Guid id, UpdateBrandRequest request);

    Task<Result<bool>> DeleteAsync(Guid id);
    Task<Result<BrandLogoutDto>> GetBrandLogout(Guid id);
}
