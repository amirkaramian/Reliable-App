using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Departments.Events;

namespace MyReliableSite.Application.Departments.EventHandlers;

public class DepartmentCreatedEventHandler : INotificationHandler<EventNotification<DepartmentCreatedEvent>>
{
    private readonly ILogger<DepartmentCreatedEventHandler> _logger;

    public DepartmentCreatedEventHandler(ILogger<DepartmentCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<DepartmentCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
