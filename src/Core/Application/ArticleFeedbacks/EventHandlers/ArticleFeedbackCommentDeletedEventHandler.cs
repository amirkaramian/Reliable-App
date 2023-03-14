using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackCommentDeletedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackCommentDeletedEvent>>
{
    private readonly ILogger<ArticleFeedbackCommentDeletedEventHandler> _logger;

    public ArticleFeedbackCommentDeletedEventHandler(ILogger<ArticleFeedbackCommentDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackCommentDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
