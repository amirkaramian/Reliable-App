using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackUpdatedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackUpdatedEvent>>
{
    private readonly ILogger<ArticleFeedbackUpdatedEventHandler> _logger;

    public ArticleFeedbackUpdatedEventHandler(ILogger<ArticleFeedbackUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
