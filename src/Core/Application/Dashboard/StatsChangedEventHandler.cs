using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Domain.Dashboard;
using MyReliableSite.Shared.DTOs.Notifications;

namespace MyReliableSite.Application.Dashboard;

public class StatsChangedEventHandler : INotificationHandler<EventNotification<StatsChangedEvent>>
{
    private readonly ILogger<StatsChangedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public StatsChangedEventHandler(ILogger<StatsChangedEventHandler> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Handle(EventNotification<StatsChangedEvent> notification, CancellationToken cancellationToken)
    {
        await _notificationService.SendMessageAsync(new StatsChangedNotification());
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
    }
}