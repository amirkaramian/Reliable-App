using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Brands;

namespace MyReliableSite.Application.Brands.EventHandlers;

public class BrandUpdatedEventHandler : INotificationHandler<EventNotification<BrandUpdatedEvent>>
{
    private readonly ILogger<BrandUpdatedEventHandler> _logger;

    public BrandUpdatedEventHandler(ILogger<BrandUpdatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<BrandUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}