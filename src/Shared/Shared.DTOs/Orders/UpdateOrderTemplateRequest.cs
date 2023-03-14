using MyReliableSite.Shared.DTOs.Products;
using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.Orders;
public class UpdateOrderTemplateRequest : IMustBeValid
{
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public bool IsActive { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public FileUploadRequest Thumbnail { get; set; }
    public IList<UpdateOrderTemplateLineItemRequest> OrderTemplateLineItems { get; set; }
    public PaymentType PaymentType { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public string Notes { get; set; }
}