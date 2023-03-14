using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.Products;

public class UpdateProductRequest : IMustBeValid
{
    public string Name { get; set; }
    public string Description { get; set; }
    public FileUploadRequest Thumbnail { get; set; }
    public IList<UpdateProductLineItemRequest> ProductLineItems { get; set; }
    public ProductStatus Status { get; set; }
    public string Tags { get; set; } // Comma Seperated
    public PaymentType PaymentType { get; set; }
    public BillingCycle BillingCycle { get; set; }

    public string Notes { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? NextDueDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public DateTime? OverrideSuspensionDate { get; set; }
    public DateTime? OverrideTerminationDate { get; set; }
    public string AssignedToClientId { get; set; }
    public string DedicatedIP { get; set; }
    public string AssginedIPs { get; set; }
}