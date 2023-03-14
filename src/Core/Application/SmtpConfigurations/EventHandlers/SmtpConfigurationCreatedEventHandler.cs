using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.SmtpConfigurations;

namespace MyReliableSite.Application.SmtpConfigurations.EventHandlers;

public class SmtpConfigurationCreatedEventHandler : INotificationHandler<EventNotification<SmtpConfigurationCreatedEvent>>
{
    private readonly ILogger<SmtpConfigurationCreatedEventHandler> _logger;

    public SmtpConfigurationCreatedEventHandler(ILogger<SmtpConfigurationCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<SmtpConfigurationCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
