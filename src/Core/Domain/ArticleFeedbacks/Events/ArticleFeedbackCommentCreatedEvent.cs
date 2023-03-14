using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackCommentCreatedEvent : DomainEvent
{
    public ArticleFeedbackCommentCreatedEvent(ArticleFeedbackComment articleFeedbackComment)
    {
        ArticleFeedbackComment = articleFeedbackComment;
    }

    public ArticleFeedbackComment ArticleFeedbackComment { get; }
}
