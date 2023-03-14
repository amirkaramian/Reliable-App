using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Shared.DTOs.Products;

public class ProductEXL
{
    public Guid Id { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductNo { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public ProductStatus Status { get; set; }

    // public string Tags { get; set; } // Comma Seperated
    public PaymentType PaymentType { get; set; }
    public BillingCycle BillingCycle { get; set; }

    public string AdminAssigned { get; set; }
    public string Notes { get; set; }
    public string RegistrationDate { get; set; }
    public string NextDueDate { get; set; }
    public Guid CreatedBy { get; set; }
    public string CreatedOn { get; set; }
    public Guid LastModifiedBy { get; set; }
    public string LastModifiedOn { get; set; }
    public string DeletedOn { get; set; }
    public Guid DeletedBy { get; set; }
    public Guid AdminAsClient { get; set; }
    public string TerminationDate { get; set; }
    public string OverrideSuspensionDate { get; set; }
    public string OverrideTerminationDate { get; set; }
    public string SuspendedReason { get; set; }
    public string Tenant { get; set; }
    public string AssignedToClientId { get; set; }
    public string OldOrderId { get; set; }
    public string OldProductId { get; set; }
    public string ServerId { get; set; }
    public string DomainName { get; set; }
    public string DedicatedIP { get; set; }
    public string AssginedIPs { get; set; }
    public Guid OrderId { get; set; }

}
