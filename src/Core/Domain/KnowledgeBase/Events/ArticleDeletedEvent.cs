using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.KnowledgeBase.Events;

public class ArticleDeletedEvent : DomainEvent
{
    public ArticleDeletedEvent(Article article)
    {
        Article = article;
    }

    public Article Article { get; }
}