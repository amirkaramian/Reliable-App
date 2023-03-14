using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.KnowledgeBase;

public class UpdateArticleRequest : IMustBeValid
{
    public string Title { get; set; }
    public string BodyText { get; set; }
    public bool Visibility { get; set; }
    public string ArticleStatus { get; set; }
    public FileUploadRequest Image { get; set; }
    public List<Guid> Categories { get; set; }
    public List<Guid> BrandIds { get; set; }
}