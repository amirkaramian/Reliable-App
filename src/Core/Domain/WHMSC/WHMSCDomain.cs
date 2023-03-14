using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.WHMSC;

public class WHMSCDomain : AuditableEntity, IMustHaveTenant
{
    public string Tenant { get; set; }
    public int ID { get; set; }
    public int UserID { get; set; }
    public string ClientName { get; set; }
    public string OrderID { get; set; }
    public string RegType { get; set; }
    public string DomainName { get; set; }
    public decimal FirstPayment { get; set; }
    public decimal RecurringAmount { get; set; }
    public int RegistrationPeriod { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime NextDueDate { get; set; }
    public string Register { get; set; }
    public string PaymentMethod { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
}
