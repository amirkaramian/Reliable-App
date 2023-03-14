using MyReliableSite.Domain.Billing;
using MyReliableSite.Domain.Common;
using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Shared.DTOs.Products;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyReliableSite.Domain.Products;

public class Product : AuditableEntity, IMustHaveTenant
{
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
    public DateTime? RegistrationDate { get; set; }
    public DateTime? NextDueDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public DateTime? OverrideSuspensionDate { get; set; }
    public DateTime? OverrideTerminationDate { get; set; }
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
    public virtual Order Order { get; set; }
    public Guid? Module { get; set; }
    public AutoSetup ProductSetup { get; set; } = AutoSetup.DontSetup;
    public string ExtraData { get; set; }
    public string ModuleName { get; set; }

    // public virtual ICollection<ProductCategory> ProductCategories { get; set; } = new List<ProductCategory>();
    public virtual ICollection<ProductDepartments> ProductDepartments { get; set; } = new List<ProductDepartments>();
    public virtual ICollection<ProductLineItems> ProductLineItems { get; set; } = new List<ProductLineItems>();

    public Product()
    {

    }

    public Product(
        string name,
        string description,
        string thumbnail,
        ProductStatus status,
        PaymentType paymentType,
        string notes,
        DateTime? registrationDate,
        DateTime? nextDueDate,
        DateTime? terminationDate,
        DateTime? overrideSuspensionDate,
        DateTime? overrideTerminationDate,
        string assignedToClientId,
        BillingCycle billingCycle,
        string dedicatedIP,
        string assginedIPs)
    {
        Name = name;
        Description = description;
        Thumbnail = thumbnail;
        Status = status;
        PaymentType = paymentType;
        Notes = notes;
        RegistrationDate = registrationDate;
        NextDueDate = nextDueDate;
        TerminationDate = terminationDate;
        OverrideSuspensionDate = overrideSuspensionDate;
        OverrideTerminationDate = overrideTerminationDate;
        AssignedToClientId = assignedToClientId;
        BillingCycle = billingCycle;
        DedicatedIP = dedicatedIP;
        AssginedIPs = assginedIPs;
    }

    public Product Update(
        string name,
        string description,
        string thumbnail,
        ProductStatus status,
        PaymentType paymentType,
        string notes,
        DateTime? registrationDate,
        DateTime? nextDueDate,
        DateTime? terminationDate,
        DateTime? overrideSuspensionDate,
        DateTime? overrideTerminationDate,
        string assignedToClientId,
        BillingCycle billingCycle,
        string dedicatedIP,
        string assginedIPs)
    {
        if (name != null && !Name.NullToString().Equals(name)) Name = name;
        if (description != null && !Description.NullToString().Equals(description)) Description = description;
        if (thumbnail != null && !Thumbnail.NullToString().Equals(thumbnail)) Thumbnail = thumbnail;
        if (status != Status) Status = status;
        if (paymentType != PaymentType) PaymentType = paymentType;
        if (notes != null && !Notes.NullToString().Equals(notes)) Notes = notes;
        if (registrationDate != RegistrationDate) RegistrationDate = registrationDate;
        if (nextDueDate != NextDueDate) NextDueDate = nextDueDate;
        if (terminationDate != TerminationDate) TerminationDate = terminationDate;
        if (overrideSuspensionDate != OverrideSuspensionDate) OverrideSuspensionDate = overrideSuspensionDate;
        if (overrideTerminationDate != OverrideTerminationDate) OverrideTerminationDate = overrideTerminationDate;
        LastModifiedOn = DateTime.UtcNow;
        if (assignedToClientId != null && !AssignedToClientId.NullToString().Equals(assignedToClientId)) AssignedToClientId = assignedToClientId;
        if (BillingCycle != billingCycle) BillingCycle = billingCycle;
        if (dedicatedIP != null && !DedicatedIP.NullToString().Equals(dedicatedIP)) DedicatedIP = dedicatedIP;
        if (assginedIPs != null && !AssginedIPs.NullToString().Equals(assginedIPs)) AssginedIPs = assginedIPs;

        return this;
    }

    public Product Update(string assignedToClientId)
    {
        AssignedToClientId = assignedToClientId;
        return this;
    }

    public Product Update(string assignedAdmin, bool adminUpdate = true)
    {
        AdminAssigned = assignedAdmin;
        return this;
    }
}
