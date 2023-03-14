using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.EmailTemplates;

namespace MyReliableSite.Application.EmailTemplates.EventHandlers;

public class EmailTemplateUpdatedEventHandler : INotificationHandler<EventNotification<EmailTemplateDeletedEvent>>
{
    private readonly ILogger<EmailTemplateUpdatedEventHandler> _logger;

    public EmailTemplateUpdatedEventHandler(ILogger<EmailTemplateUpdatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<EmailTemplateDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
