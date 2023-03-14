using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackCommentCreatedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackCommentCreatedEvent>>
{
    private readonly ILogger<ArticleFeedbackCommentCreatedEventHandler> _logger;

    public ArticleFeedbackCommentCreatedEventHandler(ILogger<ArticleFeedbackCommentCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackCommentCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
