using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.KnowledgeBase.Events;

public class ArticleCreatedEvent : DomainEvent
{
    public ArticleCreatedEvent(Article article)
    {
        Article = article;
    }

    public Article Article { get; }
}