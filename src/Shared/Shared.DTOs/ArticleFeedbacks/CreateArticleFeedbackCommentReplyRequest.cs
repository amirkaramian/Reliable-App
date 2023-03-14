namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class CreateArticleFeedbackCommentReplyRequest : IMustBeValid
{
    public string CommentText { get; set; }
    public Guid? ArticleFeedbackCommentParentReplyId { get; set; }
    public Guid ArticleFeedbackCommentId { get; set; }
}