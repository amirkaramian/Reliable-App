using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackCommentReplyUpdatedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackCommentReplyUpdatedEvent>>
{
    private readonly ILogger<ArticleFeedbackCommentReplyUpdatedEventHandler> _logger;

    public ArticleFeedbackCommentReplyUpdatedEventHandler(ILogger<ArticleFeedbackCommentReplyUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackCommentReplyUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
