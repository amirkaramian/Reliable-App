using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.Products;

public class CreateProductRequest : IMustBeValid
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public Guid OrderId { get; set; }
    public string AdminAssigned { get; set; }
    public string AssignedToClientId { get; set; }
    public FileUploadRequest Thumbnail { get; set; }
    public IList<CreateProductLineItemRequest> ProductLineItems { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Active;
    public string Tags { get; set; } // Comma Seperated
    public PaymentType PaymentType { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public string Notes { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? NextDueDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public DateTime? OverrideSuspensionDate { get; set; }
    public DateTime? OverrideTerminationDate { get; set; }
    public string DedicatedIP { get; set; }
    public string AssginedIPs { get; set; }
    public object ExtraData { get; set; } = new object();
}

public class CreateProductRequestWHMCS
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public Guid OrderId { get; set; }
    public string AdminAssigned { get; set; }
    public string AssignedToClientId { get; set; }
    public FileUploadRequest Thumbnail { get; set; }
    public List<CreateProductLineItemRequest> ProductLineItems { get; set; }
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
    public string OldOrderId { get; set; }
    public string OldProductId { get; set; }
    public string ServerId { get; set; }
    public string DomainName { get; set; }
    public string DedicatedIP { get; set; }
    public string AssginedIPs { get; set; }
    public decimal RecurringAmount { get; set; }
    public DateTime ExpiryDate { get; set; }
    public object ExtraData { get; set; } = new object();
}
