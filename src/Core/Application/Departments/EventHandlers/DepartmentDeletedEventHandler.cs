using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.Departments.Events;

namespace MyReliableSite.Application.Departments.EventHandlers;

public class DepartmentDeletedEventHandler : INotificationHandler<EventNotification<DepartmentDeletedEvent>>
{
    private readonly ILogger<DepartmentDeletedEventHandler> _logger;

    public DepartmentDeletedEventHandler(ILogger<DepartmentDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<DepartmentDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
