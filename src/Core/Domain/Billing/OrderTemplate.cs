using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Products;

namespace MyReliableSite.Domain.Billing;

public class OrderTemplate : AuditableEntity, IMustHaveTenant
{
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }

    // public ProductStatus Status { get; set; }
    public string Tenant { get; set; }
    public string Description { get; set; }
    public string Notes { get; set; }
    public string Name { get; set; }
    public decimal Total { get; set; }
    public virtual ICollection<OrderTemplateLineItem> OrderTemplateLineItems { get; set; } = new List<OrderTemplateLineItem>();
    public bool IsActive { get; set; }
    public string Thumbnail { get; set; }

    // public string Tags { get; set; } // Comma Seperated
    public PaymentType PaymentType { get; set; }
    public BillingCycle BillingCycle { get; set; }
    public string UserId { get; set; }

    // public virtual ICollection<OrderTemplateCategory> OrderTemplateCategories { get; set; } = new List<OrderTemplateCategory>();
    // public virtual ICollection<OrderTemplateDepartments> OrderTemplateDepartments { get; set; } = new List<OrderTemplateDepartments>();

    public OrderTemplate()
    {

    }

    public OrderTemplate(
        string productName,
        string productDescription,
        bool isActive,
        string name,
        string description,
        string thumbnail,
        PaymentType paymentType,
        string userId,
        BillingCycle billingCycle,
        string notes)
    {
        ProductName = productName;
        ProductDescription = productDescription;
        IsActive = isActive;
        Name = name;
        Description = description;
        Thumbnail = thumbnail;
        PaymentType = paymentType;
        UserId = userId;
        BillingCycle = billingCycle;
        Notes = notes;
    }

    public OrderTemplate Update(
        string productName,
        string productDescription,
        bool isActive,
        string name,
        string description,
        string thumbnail,
        PaymentType paymentType,
        string userId,
        BillingCycle billingCycle,
        string notes)
    {
        if (productDescription != null && !ProductDescription.NullToString().Equals(productDescription)) ProductDescription = productDescription;
        if (productName != null && !ProductName.NullToString().Equals(productName)) ProductName = productName;

        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        if (thumbnail != null && !Thumbnail.NullToString().Equals(thumbnail)) Thumbnail = thumbnail;
        if (isActive != IsActive) IsActive = isActive;
        if (paymentType != PaymentType) PaymentType = paymentType;
        if (userId != null && !UserId.NullToString().Equals(userId)) UserId = userId;
        LastModifiedOn = DateTime.UtcNow;
        if (BillingCycle != billingCycle) BillingCycle = billingCycle;
        if (notes != null && !Notes.NullToString().Equals(notes)) Notes = notes;
        return this;
    }
}

public class OrderTemplateLineItem : AuditableEntity
{
    public string LineItem { get; set; }
    public decimal Price { get; set; }
    public Guid OrderTemplateId { get; set; }
    public PriceType PriceType { get; set; }
    public virtual OrderTemplate OrderTemplate { get; set; }

    public OrderTemplateLineItem()
    {
    }

    public OrderTemplateLineItem(string name, decimal price, PriceType priceType)
    {
        LineItem = name;
        Price = price;
        PriceType = priceType;
    }

    public OrderTemplateLineItem(string name, decimal price, Guid orderTemplateId, PriceType priceType)
    {
        LineItem = name;
        Price = price;
        OrderTemplateId = orderTemplateId;
        PriceType = priceType;
    }

    public OrderTemplateLineItem Update(string name, decimal price, PriceType priceType)
    {
        if (name != null && !LineItem.NullToString().Equals(name)) LineItem = name;
        if (Price != price) Price = price;
        if (PriceType != priceType) PriceType = priceType;
        return this;
    }
}
