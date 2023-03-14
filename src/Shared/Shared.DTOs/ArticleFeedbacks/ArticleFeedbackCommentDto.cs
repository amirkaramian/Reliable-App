namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class ArticleFeedbackCommentDto : IDto
{
    public Guid Id { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get;  set; }
    public string UserFullName { get; set; }
    public string UserImagePath { get; set; }
    public List<ArticleFeedbackCommentReplyDto> ArticleFeedbackCommentReplies { get; set; } = new List<ArticleFeedbackCommentReplyDto>();
}

