using MyReliableSite.Shared.DTOs.Brands;
using MyReliableSite.Shared.DTOs.Categories;

namespace MyReliableSite.Shared.DTOs.KnowledgeBase;

public class ArticleDetailsDto : IDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string BodyText { get; set; }
    public string ImagePath { get; set; }
    public bool Visibility { get; set; }
    public string ArticleStatus { get; set; }
    public List<BrandArticleDto> BrandArticles { get; set; }
    public List<ArticleCategoryDto> ArticleCategories { get; set; }
    public string Base64Image { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class BrandArticleDto : IDto
{
    public Guid BrandId { get; set; }
    public BrandDto Brand { get; set; }
}

public class ArticleCategoryDto : IDto
{
    public Guid CategoryId { get; set; }
    public CategoryDto Category { get; set; }
}