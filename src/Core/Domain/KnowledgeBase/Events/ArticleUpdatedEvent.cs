using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.KnowledgeBase.Events;

public class ArticleUpdatedEvent : DomainEvent
{
    public ArticleUpdatedEvent(Article article)
    {
        Article = article;
    }

    public Article Article { get; }
}