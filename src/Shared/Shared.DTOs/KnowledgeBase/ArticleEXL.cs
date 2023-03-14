namespace MyReliableSite.Shared.DTOs.KnowledgeBase;

public class ArticleEXL
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string BodyText { get; set; }
    public string Tenant { get; set; }
    public string ImagePath { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public Guid DeletedBy { get; set; }
    public Guid AdminAsClient { get; set; }
    public string LastModifiedOn { get; set; }
    public string DeletedOn { get; set; }
    public bool Visibility { get; set; }
    public string ArticleStatus { get; set; }
    public int Views { get; set; } = 0;
}
