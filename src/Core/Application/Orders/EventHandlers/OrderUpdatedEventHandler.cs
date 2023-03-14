using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Orders;

namespace MyReliableSite.Application.Orders.EventHandlers;

public class OrderUpdatedEventHandler : INotificationHandler<EventNotification<OrderUpdatedEvent>>
{
    private readonly ILogger<OrderUpdatedEventHandler> _logger;

    public OrderUpdatedEventHandler(ILogger<OrderUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<OrderUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
