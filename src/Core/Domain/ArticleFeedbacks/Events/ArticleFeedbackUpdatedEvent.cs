using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackUpdatedEvent : DomainEvent
{
    public ArticleFeedbackUpdatedEvent(ArticleFeedback articleFeedback)
    {
        ArticleFeedback = articleFeedback;
    }

    public ArticleFeedback ArticleFeedback { get; }
}
