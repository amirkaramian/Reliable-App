namespace MyReliableSite.Shared.DTOs.KnowledgeBase;

public class ArticleDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string BodyText { get; set; }
    public string ImagePath { get; set; }
    public bool Visibility { get; set; }
    public string ArticleStatus { get; set; }
    public List<ArticleCategoryDto> ArticleCategories { get; set; }
    public List<BrandArticleDto> BrandArticles { get; set; }
    public string Base64Image { get; set; }
    public DateTime CreatedOn { get; set; }
    public int Views { get; set; }
}