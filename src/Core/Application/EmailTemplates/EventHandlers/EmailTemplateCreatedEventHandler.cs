using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.EmailTemplates;

namespace MyReliableSite.Application.EmailTemplates.EventHandlers;

public class EmailTemplateCreatedEventHandler : INotificationHandler<EventNotification<EmailTemplateCreatedEvent>>
{
    private readonly ILogger<EmailTemplateCreatedEventHandler> _logger;

    public EmailTemplateCreatedEventHandler(ILogger<EmailTemplateCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<EmailTemplateCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
