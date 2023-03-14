using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.SmtpConfigurations;

namespace MyReliableSite.Application.SmtpConfigurations.Interfaces;

public interface ISmtpConfigurationService : ITransientService
{
    Task<Result<SmtpConfigurationDto>> GetAsync(Guid id);

    Task<Result<SmtpConfigurationDto>> GetByBrandAsync(Guid brandId);

    Task<Result<IEnumerable<SmtpConfigurationDto>>> GetAllAsync();

    Task<PaginatedResult<SmtpConfigurationDto>> SearchAsync(SmtpConfigurationListFilter filter);

    Task<Result<SmtpConfigurationDto>> CreateAsync(CreateSmtpConfigurationRequest request);

    Task<Result<SmtpConfigurationDto>> UpdateAsync(Guid id, UpdateSmtpConfigurationRequest request);

    Task<Result<SmtpConfigurationDto>> UpdateForBrandAsync(Guid brandId, Guid departmentId, UpdateSmtpConfigurationRequest request);

    Task<Result<bool>> DeleteAsync(Guid id);

    Task<Result<bool>> DeleteForBrandAsync(Guid brandId);
}
