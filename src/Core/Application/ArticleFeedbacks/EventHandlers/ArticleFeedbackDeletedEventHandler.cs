using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.ArticleFeedbacks.Events;

namespace MyReliableSite.Application.ArticleFeedbacks.EventHandlers;

public class ArticleFeedbackDeletedEventHandler : INotificationHandler<EventNotification<ArticleFeedbackDeletedEvent>>
{
    private readonly ILogger<ArticleFeedbackDeletedEventHandler> _logger;

    public ArticleFeedbackDeletedEventHandler(ILogger<ArticleFeedbackDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleFeedbackDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
