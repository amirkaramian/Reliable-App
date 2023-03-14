using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackCommentDeletedEvent : DomainEvent
{
    public ArticleFeedbackCommentDeletedEvent(ArticleFeedbackComment articleFeedbackComment)
    {
        ArticleFeedbackComment = articleFeedbackComment;
    }

    public ArticleFeedbackComment ArticleFeedbackComment { get; }
}
