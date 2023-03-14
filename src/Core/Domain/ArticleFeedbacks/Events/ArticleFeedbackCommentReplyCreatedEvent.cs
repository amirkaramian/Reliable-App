using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks.Events;

public class ArticleFeedbackCommentReplyCreatedEvent : DomainEvent
{
    public ArticleFeedbackCommentReplyCreatedEvent(ArticleFeedbackCommentReply articleFeedbackCommentReply)
    {
        ArticleFeedbackCommentReply = articleFeedbackCommentReply;
    }

    public ArticleFeedbackCommentReply ArticleFeedbackCommentReply { get; }
}
