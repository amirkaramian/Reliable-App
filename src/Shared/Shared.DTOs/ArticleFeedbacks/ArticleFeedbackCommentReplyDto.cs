namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class ArticleFeedbackCommentReplyDto : IDto
{
    public Guid Id { get; set; }
    public string CommentText { get; set; }
    public string UserId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string UserFullName { get; set; }
    public string UserImagePath { get; set; }
    public Guid? ArticleFeedbackCommentParentReplyId { get; set; }

    // public ICollection<ArticleFeedbackCommentReplyDto> ArticleFeedbackCommentReplies { get; set; }
    // public ArticleFeedbackCommentDto ArticleFeedbackComment { get; set; }
}
