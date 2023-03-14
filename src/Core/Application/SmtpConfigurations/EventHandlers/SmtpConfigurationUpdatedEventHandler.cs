using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.SmtpConfigurations;

namespace MyReliableSite.Application.SmtpConfigurations.EventHandlers;

public class SmtpConfigurationUpdatedEventHandler : INotificationHandler<EventNotification<SmtpConfigurationUpdatedEvent>>
{
    private readonly ILogger<SmtpConfigurationUpdatedEventHandler> _logger;

    public SmtpConfigurationUpdatedEventHandler(ILogger<SmtpConfigurationUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<SmtpConfigurationUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
