using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.KnowledgeBase.Events;

namespace MyReliableSite.Application.KnowledgeBase.EventHandlers;

public class ArticleDeletedEventHandler : INotificationHandler<EventNotification<ArticleDeletedEvent>>
{
    private readonly ILogger<ArticleDeletedEventHandler> _logger;

    public ArticleDeletedEventHandler(ILogger<ArticleDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}