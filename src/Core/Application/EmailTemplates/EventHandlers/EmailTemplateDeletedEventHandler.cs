using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.EmailTemplates;

namespace MyReliableSite.Application.EmailTemplates.EventHandlers;

public class EmailTemplateDeletedEventHandler : INotificationHandler<EventNotification<EmailTemplateUpdatedEvent>>
{
    private readonly ILogger<EmailTemplateDeletedEventHandler> _logger;

    public EmailTemplateDeletedEventHandler(ILogger<EmailTemplateDeletedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<EmailTemplateUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
