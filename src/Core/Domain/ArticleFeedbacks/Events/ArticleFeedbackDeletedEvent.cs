using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackDeletedEvent : DomainEvent
{
    public ArticleFeedbackDeletedEvent(ArticleFeedback articleFeedback)
    {
        ArticleFeedback = articleFeedback;
    }

    public ArticleFeedback ArticleFeedback { get; }
}
