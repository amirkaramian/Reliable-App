using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.Brands;

namespace MyReliableSite.Application.Brands.EventHandlers;

public class BrandCreatedEventHandler : INotificationHandler<EventNotification<BrandCreatedEvent>>
{
    private readonly ILogger<BrandCreatedEventHandler> _logger;

    public BrandCreatedEventHandler(ILogger<BrandCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<BrandCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
