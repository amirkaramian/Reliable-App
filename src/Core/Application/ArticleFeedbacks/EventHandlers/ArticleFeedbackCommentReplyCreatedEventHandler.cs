using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackCommentReplyCreatedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackCommentReplyCreatedEvent>>
{
    private readonly ILogger<ArticleFeedbackCommentReplyCreatedEventHandler> _logger;

    public ArticleFeedbackCommentReplyCreatedEventHandler(ILogger<ArticleFeedbackCommentReplyCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackCommentReplyCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
