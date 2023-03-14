using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackCommentReplyUpdatedEvent : DomainEvent
{
    public ArticleFeedbackCommentReplyUpdatedEvent(ArticleFeedbackCommentReply articleFeedbackCommentReply)
    {
        ArticleFeedbackCommentReply = articleFeedbackCommentReply;
    }

    public ArticleFeedbackCommentReply ArticleFeedbackCommentReply { get; }
}
