using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ManageModule.Events;

namespace MyReliableSite.Application.ManageModule.EventHandlers;

public class ModuleDeletedEventHandler : INotificationHandler<EventNotification<ModuleDeletedEvent>>
{
    private readonly ILogger<ModuleDeletedEventHandler> _logger;

    public ModuleDeletedEventHandler(ILogger<ModuleDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ModuleDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
