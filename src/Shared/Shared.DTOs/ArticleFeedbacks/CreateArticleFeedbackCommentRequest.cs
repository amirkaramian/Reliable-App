namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class CreateArticleFeedbackCommentRequest : IMustBeValid
{
    public Guid ArticleFeedbackId { get; set; }
    public string CommentText { get; set; }
}
