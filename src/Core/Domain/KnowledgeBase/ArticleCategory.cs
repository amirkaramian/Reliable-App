using MyReliableSite.Domain.Categories;
using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.KnowledgeBase;

public class ArticleCategory : AuditableEntity
{
    public Article Article { get; set; }
    public Guid ArtilceId { get; set; } = Guid.Empty;
    public Guid ArticleId { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
}
