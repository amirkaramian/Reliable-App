namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class UpdateArticleFeedbackCommentRequest : IMustBeValid
{
    public string CommentText { get; set; }
}
