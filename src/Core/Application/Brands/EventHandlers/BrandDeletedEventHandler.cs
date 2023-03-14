using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Brands;

namespace MyReliableSite.Application.Brands.EventHandlers;

public class BrandDeletedEventHandler : INotificationHandler<EventNotification<BrandDeletedEvent>>
{
    private readonly ILogger<BrandDeletedEventHandler> _logger;

    public BrandDeletedEventHandler(ILogger<BrandDeletedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<BrandDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}