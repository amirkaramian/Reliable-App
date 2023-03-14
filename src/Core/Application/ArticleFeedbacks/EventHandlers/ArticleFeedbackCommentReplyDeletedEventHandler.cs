using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackCommentReplyDeletedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackCommentReplyDeletedEvent>>
{
    private readonly ILogger<ArticleFeedbackCommentReplyDeletedEventHandler> _logger;

    public ArticleFeedbackCommentReplyDeletedEventHandler(ILogger<ArticleFeedbackCommentReplyDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackCommentReplyDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
