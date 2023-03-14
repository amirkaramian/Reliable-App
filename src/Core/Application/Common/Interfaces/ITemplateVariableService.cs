using MyReliableSite.Application.Wrapper;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Application.Common.Interfaces;

public interface ITemplateVariableService
{
    Task<Result<TemplateVariableDto>> GetAsync(Guid id);

    Task<PaginatedResult<TemplateVariableDto>> SearchAsync(PaginationFilter filter);

    Task<Result<TemplateVariableDto>> CreateAsync(CreateTemplateVariableRequest request);

    Task<Result<TemplateVariableDto>> UpdateAsync(Guid id, UpdateTemplateVariableRequest request);

    Task<Result<bool>> DeleteAsync(Guid id);
}
