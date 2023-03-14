using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.WebHooksDomain;
using MyReliableSite.Shared.DTOs.WebHooks;

namespace MyReliableSite.Application.WebHooks.Interfaces;

public interface IWebHooksService : ITransientService
{
    Task<PaginatedResult<WebHooksDetailsDto>> SearchWebHookAsync(WebHookListFilter filter);
    Task<Result<Guid>> CreateWebHooksAsync(CreateWebHooksRequest request);

    Task<Result<Guid>> UpdateWebHooksAsync(UpdateWebHooksRequest request, Guid id);

    Task<Result<WebHooksDetailsDto>> GetWebHooksDetailsAsync(Guid id);

    Task<Result<Guid>> DeleteWebHooksAsync(Guid id);
}