using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.WHMSC;

public class WHMSCClient : AuditableEntity, IMustHaveTenant
{
    public string Tenant { get; set; }
    public string ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string CompanyName { get; set; }
    public string Email { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Postcode { get; set; }
    public string Country { get; set; }
    public string PhoneNumber { get; set; }
    public decimal Currency { get; set; }
    public string ClientGroupID { get; set; }
    public decimal Credit { get; set; }
    public DateTime CreationDate { get; set; }
    public string Notes { get; set; }
    public string Status { get; set; }
}
