using MyReliableSite.Shared.DTOs.KnowledgeBase;

namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class ArticleFeedbackDto : IDto
{
    public Guid Id { get; set; }
    public string AssignedTo { get; set; }
    public bool IsReviewed { get; set; }
    public ArticleFeedbackStatus ArticleFeedbackStatus { get; set; }
    public ArticleFeedbackPriority ArticleFeedbackPriority { get; set; }
    public string Description { get; set; }
    public ArticleFeedbackRelatedToRequest ArticleFeedbackRelatedTo { get; set; }
    public string ArticleFeedbackRelatedToId { get; set; }
    public List<ArticleFeedbackCommentDto> ArticleFeedbackComments { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedOn { get;  set; }
    public Guid LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }
    public string UserFullName { get; set; }
    public string UserImagePath { get; set; }
    public string AssignedToFullName { get; set; }
    public string CreatedByName { get; set; }
    public ArticleDto Article { get; set; }
}

public enum ArticleFeedbackStatus
{
    Active,
    Closed,
    Disabled
}
