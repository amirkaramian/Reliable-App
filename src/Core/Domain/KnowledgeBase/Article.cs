using MyReliableSite.Domain.ArticleFeedbacks;
using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.KnowledgeBase;

public class Article : AuditableEntity, IMustHaveTenant
{
    public string Title { get; set; }
    public string BodyText { get; set; }
    public string Tenant { get; set; }
    public string ImagePath { get; set; }
    public bool Visibility { get; set; }
    public string ArticleStatus { get; set; }
    public ICollection<BrandArticle> BrandArticles { get; set; }
    public ICollection<ArticleCategory> ArticleCategories { get; set; }
    public virtual ArticleFeedback ArticleFeedbacks { get; set; }
    public int Views { get; set; } = 0;
    public Article(string title, string bodyText, bool visibility, string imagePath, string articleStatus)
    {
        Title = title;
        BodyText = bodyText;
        Visibility = visibility;
        ImagePath = imagePath;
        ArticleStatus = articleStatus;
        ArticleCategories = new List<ArticleCategory>();
        BrandArticles = new List<BrandArticle>();
    }

    protected Article()
    {
    }

    public Article Update(string title, string bodyText, bool visibility, string imagePath, string articleStatus)
    {
        if (title != null && !Title.NullToString().Equals(title)) Title = title;
        if (bodyText != null && !BodyText.NullToString().Equals(bodyText)) BodyText = bodyText;
        if (Visibility != visibility) Visibility = visibility;
        if (articleStatus != null && !ArticleStatus.NullToString().Equals(articleStatus)) ArticleStatus = articleStatus;
        if (imagePath != null && !ImagePath.NullToString().Equals(imagePath)) ImagePath = imagePath;
        return this;
    }
}
