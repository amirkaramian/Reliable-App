namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class UpdateArticleFeedbackRequest : IMustBeValid
{
    public string AssignedTo { get; set; }
    public ArticleFeedbackStatus ArticleFeedbackStatus { get; set; }
    public ArticleFeedbackPriority ArticleFeedbackPriority { get; set; }
    public string Description { get; set; }
    public bool IsReviewed { get; set; }
    public ArticleFeedbackRelatedToRequest ArticleFeedbackRelatedTo { get; set; }
    public string ArticleFeedbackRelatedToId { get; set; }
}