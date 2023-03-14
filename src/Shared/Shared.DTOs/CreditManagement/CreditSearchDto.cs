using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyReliableSite.Shared.DTOs.CreditManagement;
public class CreditSearchDto : CreditDto
{
    public DateTime DueDate { get; set; }
    [JsonPropertyName("clientId")]
    public Guid UserId { get; set; }
    [JsonPropertyName("creditTransactionType")]
    public byte CreditTransactionType { get; set; }
}
