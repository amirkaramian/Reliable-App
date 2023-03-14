using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackCommentUpdatedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackCommentUpdatedEvent>>
{
    private readonly ILogger<ArticleFeedbackCommentUpdatedEventHandler> _logger;

    public ArticleFeedbackCommentUpdatedEventHandler(ILogger<ArticleFeedbackCommentUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackCommentUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
