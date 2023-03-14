using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.Orders;
public class InvoiceDto : IDto
{
    public string UserId { get; set; }
    public string Description { get; set; }
    public decimal Amount { get; set; }
    [JsonPropertyName("Date")]
    public DateTime CreatedOn { get; set; }
    [JsonIgnore]
    public Guid Id { get; set; }
    [JsonPropertyName("InvoiceNo")]
    public string BillNo { get; set; }
    [JsonIgnore]
    public Guid OrderId { get; set; }
    [JsonIgnore]
    public Guid ProductId { get; set; }
    public string Tenant { get; set; }
}
