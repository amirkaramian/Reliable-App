using Mapster;

using Microsoft.Extensions.Localization;

using MyReliableSite.Application.PaymentGateways.Interfaces;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Storage;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Billing.Events.PaymentGateways;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.PaymentGateways;

namespace MyReliableSite.Application.PaymentGateways.Services;

public class PaymentGatewayService : IPaymentGatewayService
{
    private readonly IRepositoryAsync _repository;
    private readonly IStringLocalizer<PaymentGatewayService> _localizer;

    public PaymentGatewayService(IRepositoryAsync repository, IStringLocalizer<PaymentGatewayService> localizer)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    }

    public async Task<Result<PaymentGatewayDto>> GetAsync(Guid id)
    {
        var toReturn = await _repository.GetByIdAsync<PaymentGateway>(id);

        if (toReturn == null)
            throw new EntityNotFoundException(string.Format(_localizer["paymentGateway.notfound"], id));

        return await Result<PaymentGatewayDto>.SuccessAsync(toReturn.Adapt<PaymentGatewayDto>());
    }

    public async Task<PaginatedResult<PaymentGatewayDto>> SearchAsync(PaymentGatewayListFilter filter)
    {
        return await _repository.GetSearchResultsAsync<PaymentGateway, PaymentGatewayDto>(filter.PageNumber, filter.PageSize, filter.OrderBy, filter.OrderType, new Filters<PaymentGateway>(), filter.AdvancedSearch, filter.Keyword);
    }

    public async Task<Result<PaymentGatewayDto>> CreateAsync(CreatePaymentGatewayRequest request)
    {
        if (await _repository.ExistsAsync<PaymentGateway>(a => a.ApiKey == request.ApiKey))
            throw new EntityAlreadyExistsException(string.Format(_localizer["paymentGateway.alreadyexists"], request.ApiKey));

        var toAdd = new PaymentGateway(request.Name, request.ApiKey, request.Status);

        toAdd.DomainEvents.Add(new PaymentGatewayCreatedEvent(toAdd));
        toAdd.DomainEvents.Add(new StatsChangedEvent());

        _ = await _repository.CreateAsync(toAdd);
        _ = await _repository.SaveChangesAsync();
        return await Result<PaymentGatewayDto>.SuccessAsync(toAdd.Adapt<PaymentGatewayDto>());
    }

    public async Task<Result<PaymentGatewayDto>> UpdateAsync(Guid id, UpdatePaymentGatewayRequest request)
    {
        var toUpdate = await _repository.GetByIdAsync<PaymentGateway>(id);

        if (toUpdate == null)
            throw new EntityNotFoundException(string.Format(_localizer["paymentGateway.notfound"], id));

        var updatedPaymentGateway = toUpdate.Update(request.Name, request.ApiKey, request.Status);

        toUpdate.DomainEvents.Add(new PaymentGatewayUpdatedEvent(toUpdate));
        toUpdate.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedPaymentGateway);
        _ = await _repository.SaveChangesAsync();
        return await Result<PaymentGatewayDto>.SuccessAsync(updatedPaymentGateway.Adapt<PaymentGatewayDto>());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var toDelete = await _repository.RemoveByIdAsync<PaymentGateway>(id);

        toDelete.DomainEvents.Add(new PaymentGatewayDeletedEvent(toDelete));
        toDelete.DomainEvents.Add(new StatsChangedEvent());
        return await Result<bool>.SuccessAsync(await _repository.SaveChangesAsync() > 0);
    }
}
