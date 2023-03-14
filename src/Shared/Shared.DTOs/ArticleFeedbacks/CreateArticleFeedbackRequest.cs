namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class CreateArticleFeedbackRequest : IMustBeValid
{
    public string AssignedTo { get; set; } = string.Empty;
    public ArticleFeedbackStatus ArticleFeedbackStatus { get; set; } = ArticleFeedbackStatus.Active;
    public ArticleFeedbackPriority ArticleFeedbackPriority { get; set; } = ArticleFeedbackPriority.NotUrgent;
    public string Description { get; set; }
    public ArticleFeedbackRelatedToRequest ArticleFeedbackRelatedTo { get; set; } = ArticleFeedbackRelatedToRequest.KnowledgeBase;
    public string ArticleFeedbackRelatedToId { get; set; }
}

public enum ArticleFeedbackRelatedToRequest
{
    KnowledgeBase,
    ClientProblem
}

public enum ArticleFeedbackPriority
{
    Urgent,
    NotUrgent
}