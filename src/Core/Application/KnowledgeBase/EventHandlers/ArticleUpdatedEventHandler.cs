using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.KnowledgeBase.Events;

namespace MyReliableSite.Application.KnowledgeBase.EventHandlers;

public class ArticleUpdatedEventHandler : INotificationHandler<EventNotification<ArticleUpdatedEvent>>
{
    private readonly ILogger<ArticleUpdatedEventHandler> _logger;

    public ArticleUpdatedEventHandler(ILogger<ArticleUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}