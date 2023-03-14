using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Orders;

namespace MyReliableSite.Application.Orders.EventHandlers;

public class OrderDeletedEventHandler : INotificationHandler<EventNotification<OrderDeletedEvent>>
{
    private readonly ILogger<OrderDeletedEventHandler> _logger;

    public OrderDeletedEventHandler(ILogger<OrderDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<OrderDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
