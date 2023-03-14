using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.SmtpConfigurations;

namespace MyReliableSite.Application.SmtpConfigurations.EventHandlers;

public class SmtpConfigurationDeletedEventHandler : INotificationHandler<EventNotification<SmtpConfigurationDeletedEvent>>
{
    private readonly ILogger<SmtpConfigurationDeletedEventHandler> _logger;

    public SmtpConfigurationDeletedEventHandler(ILogger<SmtpConfigurationDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<SmtpConfigurationDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
