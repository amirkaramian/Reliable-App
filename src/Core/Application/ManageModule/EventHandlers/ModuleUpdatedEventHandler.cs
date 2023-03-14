using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ManageModule.Events;

namespace MyReliableSite.Application.ManageModule.EventHandlers;

public class ModuleUpdatedEventHandler : INotificationHandler<EventNotification<ModuleUpdatedEvent>>
{
    private readonly ILogger<ModuleUpdatedEventHandler> _logger;

    public ModuleUpdatedEventHandler(ILogger<ModuleUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ModuleUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
