using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.TemplateVariables;

namespace MyReliableSite.Application.EmailTemplates.EventHandlers;

public class TemplateVariableCreatedEventHandler : INotificationHandler<EventNotification<TemplateVariableCreatedEvent>>
{
    private readonly ILogger<TemplateVariableCreatedEventHandler> _logger;

    public TemplateVariableCreatedEventHandler(ILogger<TemplateVariableCreatedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<TemplateVariableCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
