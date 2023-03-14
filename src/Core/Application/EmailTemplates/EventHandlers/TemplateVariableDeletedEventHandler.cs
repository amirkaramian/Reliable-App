using MediatR;

using Microsoft.Extensions.Logging;

using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Billing.Events.TemplateVariables;

namespace MyReliableSite.Application.EmailTemplates.EventHandlers;

public class TemplateVariableDeletedEventHandler : INotificationHandler<EventNotification<TemplateVariableDeletedEvent>>
{
    private readonly ILogger<TemplateVariableDeletedEventHandler> _logger;

    public TemplateVariableDeletedEventHandler(ILogger<TemplateVariableDeletedEventHandler> logger) => _logger = logger;

    public Task Handle(EventNotification<TemplateVariableDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
