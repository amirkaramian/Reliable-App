using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.ArticleFeedbacks;

public class ArticleFeedbackComment : AuditableEntity
{
    public ArticleFeedback ArticleFeedback { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public ICollection<ArticleFeedbackCommentReply> ArticleFeedbackCommentReplies { get; set; }
    public ArticleFeedbackComment(string commentText, string userId)
    {
        CommentText = commentText;
        UserId = userId;
    }

    public ArticleFeedbackComment Update(string commentText)
    {
        if (commentText != null && !CommentText.NullToString().Equals(commentText)) CommentText = commentText;
        return this;
    }
}
