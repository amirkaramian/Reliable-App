using System.ComponentModel.DataAnnotations;

namespace MyReliableSite.Shared.DTOs.Billing;

public class CreditEXL
{
    public Guid Id { get; set; }
    [Range(typeof(decimal), "1", "79228162514264337593543950335")]
    public decimal Amount { get; set; }
    public string DueDate { get; set; }
    public string Tenant { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public string LastModifiedOn { get; set; }
    public string DeletedOn { get; set; }
    public Guid DeletedBy { get; set; }
    public string AdminAsClient { get; set; }
    public Guid UserId { get; set; }
    public int CreditTransactionType { get; set; }
    public string Description { get; set; }

}
