using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Localization;
using MyReliableSite.Application.Brands.Services;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.Brands;
using MyReliableSite.Domain.Billing.Events.TemplateVariables;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.EmailTemplates;
using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Infrastructure.Common.Services;

public class TemplateVariableService : ITemplateVariableService
{
    private readonly IRepositoryAsync _repository;
    private readonly IStringLocalizer<TemplateVariableService> _localizer;

    public TemplateVariableService(IRepositoryAsync repository, IStringLocalizer<TemplateVariableService> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Result<TemplateVariableDto>> GetAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<TemplateVariable>(id);

        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["templatevariable.notfound"], id));

        return await Result<TemplateVariableDto>.SuccessAsync(toReturn.Adapt<TemplateVariableDto>());
    }

    public async Task<PaginatedResult<TemplateVariableDto>> SearchAsync(PaginationFilter filter)
    {
        return await _repository.GetSearchResultsAsync<TemplateVariable, TemplateVariableDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, new Filters<TemplateVariable>(), filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<TemplateVariableDto>> CreateAsync(CreateTemplateVariableRequest request)
    {
        if (await _repository.ExistsAsync<TemplateVariable>(a => string.Equals(a.Variable, request.Variable, StringComparison.CurrentCultureIgnoreCase)))
            throw new EntityAlreadyExistsException(string.Format(_localizer["templatevariable.alreadyexists"], request.Variable));

        var toAdd = new TemplateVariable(request.Variable, request.Description);

        toAdd.DomainEvents.Add(new TemplateVariableCreatedEvent(toAdd));
        toAdd.DomainEvents.Add(new StatsChangedEvent());

        _ = await _repository.CreateAsync(toAdd);
        _ = await _repository.SaveChangesAsync();
        return await Result<TemplateVariableDto>.SuccessAsync(toAdd.Adapt<TemplateVariableDto>());
    }

    public async Task<Result<TemplateVariableDto>> UpdateAsync(Guid id, UpdateTemplateVariableRequest request)
    {
        var toUpdate = await _repository.GetByIdAsync<TemplateVariable>(id);

        if (toUpdate == null)
            throw new EntityNotFoundException(string.Format(_localizer["templatevariable.notfound"], id));

        var updatedBrand = toUpdate.Update(request.Variable, request.Description);

        toUpdate.DomainEvents.Add(new TemplateVariableUpdatedEvent(toUpdate));
        toUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedBrand);
        _ = await _repository.SaveChangesAsync();
        return await Result<TemplateVariableDto>.SuccessAsync(updatedBrand.Adapt<TemplateVariableDto>());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var toDelete = await _repository.RemoveByIdAsync<TemplateVariable>(id);

        toDelete.DomainEvents.Add(new TemplateVariableDeletedEvent(toDelete));
        toDelete.DomainEvents.Add(new StatsChangedEvent());
        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }
}
