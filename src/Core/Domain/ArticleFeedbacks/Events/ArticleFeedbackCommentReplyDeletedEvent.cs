using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackCommentReplyDeletedEvent : DomainEvent
{
    public ArticleFeedbackCommentReplyDeletedEvent(ArticleFeedbackCommentReply articleFeedbackCommentReply)
    {
        ArticleFeedbackCommentReply = articleFeedbackCommentReply;
    }

    public ArticleFeedbackCommentReply ArticleFeedbackCommentReply { get; }
}
