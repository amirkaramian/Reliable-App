using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class RecurringPayment : AuditableEntity, IMustHaveTenant
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public Guid PayCustId { get; set; }
    public string Tenant { get; set; }
    public RecurringPayment(decimal amount, string description, DateTime paymentDate, Guid payCustId)
    {
        Amount = amount;
        Description = description;
        PaymentDate = paymentDate;
        PayCustId = payCustId;
    }

    public RecurringPayment Update(decimal amount, string description, DateTime paymentDate, Guid payCustId)
    {
        if (Amount != amount) Amount = amount;
        if (payCustId != Guid.Empty && !PayCustId.NullToString().Equals(payCustId)) PayCustId = payCustId;
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        if (PaymentDate != paymentDate) PaymentDate = paymentDate;
        return this;

    }
}
