using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackCreatedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackCreatedEvent>>
{
    private readonly ILogger<ArticleFeedbackCreatedEventHandler> _logger;

    public ArticleFeedbackCreatedEventHandler(ILogger<ArticleFeedbackCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
