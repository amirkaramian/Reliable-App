using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class Feedback : AuditableEntity, IMustHaveTenant
{
    public string Description { get; set; }
    public string Tenant { get; set; }
    public Feedback(string description)
    {
        Description = description;
    }

    public Feedback Update(string description)
    {
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        return this;
    }
}
