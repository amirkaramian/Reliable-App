using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MyReliableSite.Domain.Constants.PermissionConstants;

namespace MyReliableSite.Domain.WHMSC;

public class WHMSCTransaction : AuditableEntity, IMustHaveTenant
{
    public int ID { get; set; }
    public int UserID { get; set; }

    [JsonProperty("Client Name")]
    public string ClientName { get; set; }
    public string Currency { get; set; }

    [JsonProperty("Payment Method")]
    public string PaymentMethod { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }

    [JsonProperty("Invoice ID")]
    public int InvoiceID { get; set; }

    [JsonProperty("Transaction ID")]
    public string TransactionID { get; set; }

    [JsonProperty("Amount In")]
    public decimal AmountIn { get; set; }
    public decimal Fees { get; set; }

    [JsonProperty("Amount Out")]
    public decimal AmountOut { get; set; }

    [JsonProperty("Exchange Rate")]
    public decimal ExchangeRate { get; set; }

    [JsonProperty("Refund ID")]
    public int RefundId { get; set; }
    public string Tenant { get; set; }
    public WHMSCTransaction(
        int iD,
        string transactionID,
        int userID,
        string clientName,
        string currency,
        string paymentMethod,
        DateTime date,
        string description,
        int invoiceID,
        decimal amountIn,
        decimal fees,
        decimal amountOut,
        decimal exchangeRate,
        int refundId)
    {
        ID = iD;
        TransactionID = transactionID;
        UserID = userID;
        ClientName = clientName;
        Currency = currency;
        PaymentMethod = paymentMethod;
        Date = date;
        Description = description;
        InvoiceID = invoiceID;
        AmountIn = amountIn;
        Fees = fees;
        AmountOut = amountOut;
        ExchangeRate = exchangeRate;
        RefundId = refundId;
    }

}
