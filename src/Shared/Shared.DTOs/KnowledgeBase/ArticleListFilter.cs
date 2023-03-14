using MyReliableSite.Shared.DTOs.Filters;

namespace MyReliableSite.Shared.DTOs.KnowledgeBase;

public class ArticleListFilter : PaginationFilter
{
    public Guid? CategoryId { get; set; }
    public decimal? MinimumRate { get; set; }
    public decimal? MaximumRate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid? BrandId { get; set; }
}