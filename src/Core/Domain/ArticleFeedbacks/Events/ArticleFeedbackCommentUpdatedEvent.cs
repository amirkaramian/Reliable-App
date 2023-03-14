using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackCommentUpdatedEvent : DomainEvent
{
    public ArticleFeedbackCommentUpdatedEvent(ArticleFeedbackComment articleFeedbackComment)
    {
        ArticleFeedbackComment = articleFeedbackComment;
    }

    public ArticleFeedbackComment ArticleFeedbackComment { get; }
}
