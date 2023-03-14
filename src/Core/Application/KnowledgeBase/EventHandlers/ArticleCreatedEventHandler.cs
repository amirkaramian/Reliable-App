using MediatR;
using Microsoft.Extensions.Logging;
using MyReliableSite.Application.Common.Event;
using MyReliableSite.Domain.KnowledgeBase.Events;

namespace MyReliableSite.Application.KnowledgeBase.EventHandlers;

public class ArticleCreatedEventHandler : INotificationHandler<EventNotification<ArticleCreatedEvent>>
{
    private readonly ILogger<ArticleCreatedEventHandler> _logger;

    public ArticleCreatedEventHandler(ILogger<ArticleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<ArticleCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
