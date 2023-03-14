using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ManageModule.Events;

namespace MyReliableSite.Application.ManageModule.EventHandlers;

public class ModuleCreatedEventHandler : INotificationHandler<EventNotification<ModuleCreatedEvent>>
{
    private readonly ILogger<ModuleCreatedEventHandler> _logger;

    public ModuleCreatedEventHandler(ILogger<ModuleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ModuleCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
