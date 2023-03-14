using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Scripting.Interfaces;
using MyReliableSite.Domain.Billing.Events.Orders;
using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Scripting;

namespace MyReliableSite.Application.Orders.EventHandlers;

public class OrderCreatedEventHandler : INotificationHandler<EventNotification<OrderCreatedEvent>>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IRepositoryAsync _repository;
    private readonly IScriptingService _scriptingService;
    private readonly IJobService _jobService;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger, IRepositoryAsync repository, IScriptingService scriptingService, IJobService jobService)
    {
        _logger = logger;
        _repository = repository;
        _scriptingService = scriptingService;
        _jobService = jobService;
    }

    public Task Handle(EventNotification<OrderCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        foreach(var product in notification.DomainEvent.Brand.Products)
        {
            if(product.ProductSetup == AutoSetup.OnOrderPlaced && product.ModuleName != null)
            {
                var request = new RunHooksRequest() { ProductId = product.Id, HookType = HookType.Create };
                _jobService.Enqueue(() => _scriptingService.RunHooksAsync(request));
            }
        }

        return Task.CompletedTask;
    }
}
