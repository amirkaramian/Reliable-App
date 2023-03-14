using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Shared.DTOs.ArticleFeedbacks;

namespace MyReliableSite.Domain.ArticleFeedbacks;

public class ArticleFeedback : AuditableEntity, IMustHaveTenant
{
    // Clients ArticleFeedback scenario in future. so assigned to , priority will be optional

    public string AssignedTo { get; set; } // Currently, we will only show all the users of admin in a tenant | ArticleFeedbacks assignment will be from admin in client case | And another case is admin can assign the ArticleFeedbacks to other admins
    public ArticleFeedbackStatus ArticleFeedbackStatus { get; set; }
    public ArticleFeedbackPriority ArticleFeedbackPriority { get; set; }
    public string Description { get; set; }
    public bool IsReviewed { get; set; } = false;
    public ArticleFeedbackRelatedTo ArticleFeedbackRelatedTo { get; set; } // Knowledgebase, ClientProblem, TableName | Enum
    public string ArticleFeedbackRelatedToId { get; set; } // primary keys
    public string Tenant { get; set; }
    public ICollection<ArticleFeedbackComment> ArticleFeedbackComments { get; set; }
    public ArticleFeedback(string description, string assignedTo, ArticleFeedbackPriority articleFeedbackPriority, ArticleFeedbackRelatedTo articleFeedbackRelatedTo, string articleFeedbackRelatedToId, ArticleFeedbackStatus articleFeedbackStatus)
    {
        Description = description;
        AssignedTo = assignedTo;
        ArticleFeedbackPriority = articleFeedbackPriority;
        ArticleFeedbackRelatedTo = articleFeedbackRelatedTo;
        ArticleFeedbackRelatedToId = articleFeedbackRelatedToId;
        ArticleFeedbackStatus = articleFeedbackStatus;
    }

    public ArticleFeedback Update(string description, string assignedTo, ArticleFeedbackPriority articleFeedbackPriority, ArticleFeedbackRelatedTo articleFeedbackRelatedTo, string articleFeedbackRelatedToId, ArticleFeedbackStatus articleFeedbackStatus, bool isReviewed)
    {
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        if (assignedTo != null && !AssignedTo.NullToString().Equals(assignedTo)) AssignedTo = assignedTo;
        if (articleFeedbackPriority != ArticleFeedbackPriority) ArticleFeedbackPriority = articleFeedbackPriority;
        if (articleFeedbackRelatedTo != ArticleFeedbackRelatedTo) ArticleFeedbackRelatedTo = articleFeedbackRelatedTo;
        if (articleFeedbackRelatedToId != null && !ArticleFeedbackRelatedToId.NullToString().Equals(articleFeedbackRelatedToId)) ArticleFeedbackRelatedToId = articleFeedbackRelatedToId;
        if (articleFeedbackStatus != ArticleFeedbackStatus) ArticleFeedbackStatus = articleFeedbackStatus;
        if (isReviewed != IsReviewed) IsReviewed = isReviewed;

        return this;
    }
}

public enum ArticleFeedbackStatus
{
    Active,
    Closed,
    Disabled
}

public enum ArticleFeedbackRelatedTo
{
    KnowledgeBase,
    ClientProblem
}