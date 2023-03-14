using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.TemplateVariables;

namespace MyReliableSite.Application.EmailTemplates.EventHandlers;

public class TemplateVariableUpdatedEventHandler : INotificationHandler<EventNotification<TemplateVariableUpdatedEvent>>
{
    private readonly ILogger<TemplateVariableUpdatedEventHandler> _logger;

    public TemplateVariableUpdatedEventHandler(ILogger<TemplateVariableUpdatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<TemplateVariableUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
