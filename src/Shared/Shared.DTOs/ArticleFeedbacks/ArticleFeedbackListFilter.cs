using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Shared.DTOs.ArticleFeedbacks;

public class ArticleFeedbackListFilter : PaginationFilter
{
    public ArticleFeedbackRelatedToRequest ArticleFeedbackRelatedTo { get; set; }
    public ArticleFeedbackPriority ArticleFeedbackPriority { get; set; }

    public ArticleFeedbackStatus ArticleFeedbackStatus { get; set; }

}
