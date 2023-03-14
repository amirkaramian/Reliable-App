using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks;

public class ArticleFeedbackCommentReply : AuditableEntity
{
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public Guid? ArticleFeedbackCommentParentReplyId { get; set; }
    public ArticleFeedbackCommentReply ArticleFeedbackCommentParentReply { get; set; }
    public ICollection<ArticleFeedbackCommentReply> ArticleFeedbackCommentChildReplies { get; set; }
    public ArticleFeedbackComment ArticleFeedbackComment { get; set; }
    public ArticleFeedbackCommentReply(string commentText, string userId, Guid? articleFeedbackCommentParentReplyId)
    {
        CommentText = commentText;
        UserId = userId;
        ArticleFeedbackCommentParentReplyId = articleFeedbackCommentParentReplyId;
    }

    public ArticleFeedbackCommentReply Update(string commentText, Guid? articleFeedbackCommentParentReplyId)
    {
        if (commentText != null && !CommentText.NullToString().Equals(commentText)) CommentText = commentText;
        if (articleFeedbackCommentParentReplyId != ArticleFeedbackCommentParentReplyId) ArticleFeedbackCommentParentReplyId = articleFeedbackCommentParentReplyId;
        return this;
    }
}