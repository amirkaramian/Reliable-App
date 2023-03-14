using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class PaymentGateway : AuditableEntity, IMustHaveTenant
{

    public string Name { get; set; }
    public string ApiKey { get; set; }
    public bool Status { get; set; }
    public string Tenant { get; set; }
    public PaymentGateway(string name, string apiKey, bool status)
    {
        Name = name;
        ApiKey = apiKey;
        Status = status;
    }

    public PaymentGateway Update(string name, string apiKey, bool status)
    {
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (apiKey != null && !ApiKey.NullToString().Equals(apiKey)) ApiKey = apiKey;
        if (Status != status) { Status = status; }
        return this;
    }
}
