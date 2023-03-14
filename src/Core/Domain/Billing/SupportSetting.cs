using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class SupportSetting : AuditableEntity, IMustHaveTenant
{
    public int MaxNumberOfSubCategories { get; set; }
    public bool AutoApproveNewArticles { get; set; }
    public string Tenant { get; set; }
    public SupportSetting()
    {
    }

    public SupportSetting(int maxNumberOfSubCategories, bool autoApproveNewArticles)
    {
        MaxNumberOfSubCategories = maxNumberOfSubCategories;
        AutoApproveNewArticles = autoApproveNewArticles;

    }

    public SupportSetting Update(int maxNumberOfSubCategories, bool autoApproveNewArticles)
    {
        if (MaxNumberOfSubCategories != maxNumberOfSubCategories) { MaxNumberOfSubCategories = maxNumberOfSubCategories; }
        if (AutoApproveNewArticles != autoApproveNewArticles) { AutoApproveNewArticles = autoApproveNewArticles; }
        return this;
    }

}
