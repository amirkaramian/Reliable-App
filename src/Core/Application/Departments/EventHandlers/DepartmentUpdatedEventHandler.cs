using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Departments.Events;

namespace MyReliableSite.Application.Departments.EventHandlers;

public class DepartmentUpdatedEventHandler : INotificationHandler<EventNotification<DepartmentUpdatedEvent>>
{
    private readonly ILogger<DepartmentUpdatedEventHandler> _logger;

    public DepartmentUpdatedEventHandler(ILogger<DepartmentUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<DepartmentUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
