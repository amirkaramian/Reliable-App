using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReliableSite.Domain.WHMSC;

public class WHMSCService : AuditableEntity, IMustHaveTenant
{
    public string Tenant { get; set; }
    public int ID { get; set; }

    [JsonProperty("User ID")]
    public int UserID { get; set; }

    [JsonProperty("Client Name")]
    public string ClientName { get; set; }

    [JsonProperty("Order ID")]
    public string OrderID { get; set; }

    [JsonProperty("Product ID")]
    public string ProductID { get; set; }

    [JsonProperty("Server ID")]
    public string ServerID { get; set; }

    [JsonProperty("Domain Name")]
    public string DomainName { get; set; }

    [JsonProperty("Dedicated IP")]
    public string DedicatedIP { get; set; } = "0";

    [JsonProperty("Assgined IPs")]
    public string AssginedIPs { get; set; } = "0";

    [JsonProperty("First Payment Amount")]
    public decimal FirstPaymentAmount { get; set; }

    [JsonProperty("Recurring Amount")]
    public decimal RecurringAmount { get; set; }

    [JsonProperty("Billing Cycle")]
    public string BillingCycle { get; set; }

    [JsonProperty("Next Due Date")]
    public DateTime NextDue { get; set; }

    [JsonProperty("Payment Method")]
    public string PaymentMethod { get; set; }
    public string Status { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Notes { get; set; }

    [JsonProperty("Subscription ID")]
    public string SubscriptionID { get; set; }

    [JsonProperty("Suspend Reason")]
    public string SuspendReason { get; set; }
}
