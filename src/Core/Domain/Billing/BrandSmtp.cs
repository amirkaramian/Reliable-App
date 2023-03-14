using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.KnowledgeBase;

namespace MyReliableSite.Domain.Billing;

public class BrandSmtpConfiguration : BaseEntity, IMustHaveTenant
{
    public string Tenant { get; set; }
    public Guid BrandId { get; set; }
    public Brand Brand { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid SmtpConfigurationId { get; set; }
    public SmtpConfiguration SmtpConfiguration { get; set; }
}

public class BrandArticle : BaseEntity
{
    public Guid BrandId { get; set; }
    public Brand Brand { get; set; }
    public Guid Articleid { get; set; }
    public Article Article { get; set; }
}

