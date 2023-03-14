using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class Review : AuditableEntity, IMustHaveTenant
{
    public string Description { get; set; }
    public string Tenant { get; set; }
    public Review(string description)
    {
        Description = description;
    }

    public Review Update(string description)
    {
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        return this;
    }
}
