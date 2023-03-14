namespace MyReliableSite.Shared.DTOs.Products;
public class ProductDetailDto : IDto
{
    public Guid Id { get; set; }
    public int ProductNo { get; set; }
    public Guid OrderId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Thumbnail { get; set; }
    public IList<ProductDepartmentDto> ProductDepartments { get; set; }
    public IList<ProductLineItemDto> ProductLineItems { get; set; }
    public ProductStatus Status { get; set; }
    public PaymentType PaymentType { get; set; }
    public string Notes { get; set; }
    public DateTime RegistrationDate { get; set; }
    public DateTime NextDueDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public DateTime OverrideSuspensionDate { get; set; }
    public DateTime OverrideTerminationDate { get; set; }
    public string UserId { get; set; }
    public string Base64Image { get; set; }
    public string AssignedToClientId { get; set; }
    public string AssignedClient { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public DateTime? DeletedOn { get; set; }
    public Guid? DeletedBy { get; set; }
    public string DedicatedIP { get; set; }
    public string AssginedIPs { get; set; }
    public Guid InvoiceId { get; set; }
    public AutoSetup ProductSetup { get; set; }
    public Guid? Module { get; set; }
    public object FieldData { get; set; }
    public string ModuleName { get; set; }
}

public enum ProductStatus
{
    Pending = 0,
    Active = 1,
    Cancelled = 2,
    Suspended = 3
}

public enum PaymentType
{
    OneTime = 0,
    Recurring = 1
}

public enum BillingCycle
{
    Hourly = 0,
    Monthly = 1,
    Quarterly = 2,
    SemiAnnually = 3,
    Annually = 4,
    Biennially = 5,
    Triennially = 6
}

public enum AutoSetup
{
    DontSetup = 0,
    OnOrderPlaced = 1,
    OnPaymentReceived = 2,
    OnManualAccept = 3,
}
