using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackCreatedEvent : DomainEvent
{
    public ArticleFeedbackCreatedEvent(ArticleFeedback articleFeedback)
    {
        ArticleFeedback = articleFeedback;
    }

    public ArticleFeedback ArticleFeedback { get; }
}
