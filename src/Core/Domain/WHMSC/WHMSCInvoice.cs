using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using Newtonsoft.Json;

namespace MyReliableSite.Domain.WHMSC;

public class WHMSCInvoice : AuditableEntity, IMustHaveTenant
{
    public int ID { get; set; }

    [JsonProperty(PropertyName = "User ID")]
    public int UserID { get; set; }

    [JsonProperty(PropertyName = "Client Name")]

    public string ClientName { get; set; }

    [JsonProperty(PropertyName = "Creation Date")]

    public DateTime CreationDate { get; set; }

    [JsonProperty(PropertyName = "Due Date")]
    public DateTime DueDate { get; set; }

    [JsonProperty(PropertyName = "Date Paid")]
    public DateTime DatePaid { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Credit { get; set; }
    public decimal Tax { get; set; }
    public decimal Tax2 { get; set; }
    public decimal Total { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxRate2 { get; set; }
    public string Status { get; set; }

    [JsonProperty(PropertyName = "Payment Method")]
    public string PaymentMethod { get; set; }
    public string Notes { get; set; }
    public string Tenant { get; set; }

}
