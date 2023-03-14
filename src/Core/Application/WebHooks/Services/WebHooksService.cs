using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.WebHooks.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing.Events;
using MyReliableSite.Domain.WebHooksDomain;
using MyReliableSite.Domain.WebHooksDomain.Events;
using MyReliableSite.Shared.DTOs.WebHooks;

namespace MyReliableSite.Application.WebHooks.Services;

public class WebHooksService : IWebHooksService
{
    private readonly IRepositoryAsync _repository;

    public WebHooksService(IRepositoryAsync repository)
    {
        _repository = repository;
    }

    public async Task<PaginatedResult<WebHooksDetailsDto>> SearchWebHookAsync(WebHookListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<WebHook, WebHooksDetailsDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<Guid>> CreateWebHooksAsync(CreateWebHooksRequest request)
    {
        var hooks = new WebHook(request.WebHookUrl, request.ModuleId, request.Action, request.IsActive);
        hooks.DomainEvents.Add(new WebHookCreatedEvent(hooks));
        hooks.DomainEvents.Add(new StatsChangedEvent());
        var hooksId = await _repository.CreateAsync(hooks);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(hooksId);
    }

    public async Task<Result<Guid>> UpdateWebHooksAsync(UpdateWebHooksRequest request, Guid id)
    {
        var hooks = await _repository.GetByIdAsync<WebHook>(id, null);
        var updatedHooks = hooks.Update(request.WebHookUrl, request.ModuleId, request.Action, request.IsActive);
        updatedHooks.DomainEvents.Add(new WebHookUpdatedEvent(updatedHooks));
        updatedHooks.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync(updatedHooks);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<WebHooksDetailsDto>> GetWebHooksDetailsAsync(Guid id)
    {
        var hooks = await _repository.GetByIdAsync<WebHook, WebHooksDetailsDto>(id);
        return await Result<WebHooksDetailsDto>.SuccessAsync(hooks);
    }

    public async Task<Result<Guid>> DeleteWebHooksAsync(Guid id)
    {
        var delete = await _repository.RemoveByIdAsync<WebHook>(id);
        delete.DomainEvents.Add(new WebHookDeletedEvent(delete));
        delete.DomainEvents.Add(new StatsChangedEvent());
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

}
