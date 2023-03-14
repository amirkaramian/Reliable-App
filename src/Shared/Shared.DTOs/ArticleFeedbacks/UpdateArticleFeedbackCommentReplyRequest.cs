namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class UpdateArticleFeedbackCommentReplyRequest : IMustBeValid
{
    public string CommentText { get; set; }
    public Guid? ArticleFeedbackCommentParentReplyId { get; set; }
}