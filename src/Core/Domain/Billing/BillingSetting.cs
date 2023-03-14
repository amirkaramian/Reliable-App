using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class BillingSetting : AuditableEntity, IMustHaveTenant
{
    public int MaxNumberOfRefunds { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal MaxCreditAmount { get; set; }
    public string Tenant { get; set; }
    public decimal RefundRetainPercentage { get; set; } = 0;
    public bool AutoInvoiceGeneration { get; set; } = false;
    public int AutoInvoicePriorToDueDateInDays { get; set; } = 0;
    public bool EnableProductlevelInvoiceGen { get; set; } = false;
    public int ProductLevelInvoiceGenPriorToDueDateInDays { get; set; } = 0;
    public bool IsActiveOrPendingProducts { get; set; }
    public BillingSetting()
    {

    }

    public BillingSetting(
        int maxNumberOfRefunds,
        decimal minOrderAmount,
        decimal refundRetainPercentage,
        bool autoInvoiceGeneration,
        int autoInvoicePriorToDueDateInDays,
        bool enableProductlevelInvoiceGen,
        int productLevelInvoiceGenPriorToDueDateInDays,
        decimal maxCreditAmount,
        bool isActiveOrPendingProducts)
    {
        MaxNumberOfRefunds = maxNumberOfRefunds;
        MinOrderAmount = minOrderAmount;
        RefundRetainPercentage = refundRetainPercentage;
        AutoInvoiceGeneration = autoInvoiceGeneration;
        AutoInvoicePriorToDueDateInDays = autoInvoicePriorToDueDateInDays;
        EnableProductlevelInvoiceGen = enableProductlevelInvoiceGen;
        ProductLevelInvoiceGenPriorToDueDateInDays = productLevelInvoiceGenPriorToDueDateInDays;
        MaxCreditAmount = maxCreditAmount;
        IsActiveOrPendingProducts = isActiveOrPendingProducts;
    }

    public BillingSetting Update(
        int maxNumberOfRefunds,
        decimal minOrderAmount,
        decimal refundRetainPercentage,
        bool autoInvoiceGeneration,
        int autoInvoicePriorToDueDateInDays,
        bool enableProductlevelInvoiceGen,
        int productLevelInvoiceGenPriorToDueDateInDays,
        decimal maxCreditAmount,
        bool isActiveOrPendingProducts)
    {
        if (MaxNumberOfRefunds != maxNumberOfRefunds) { MaxNumberOfRefunds = maxNumberOfRefunds; }
        if (MinOrderAmount != minOrderAmount) { MinOrderAmount = minOrderAmount; }
        if (RefundRetainPercentage != refundRetainPercentage) { RefundRetainPercentage = refundRetainPercentage; }
        if (AutoInvoiceGeneration != autoInvoiceGeneration) { AutoInvoiceGeneration = autoInvoiceGeneration; }
        if (AutoInvoicePriorToDueDateInDays != autoInvoicePriorToDueDateInDays) { AutoInvoicePriorToDueDateInDays = autoInvoicePriorToDueDateInDays; }
        if (EnableProductlevelInvoiceGen != enableProductlevelInvoiceGen) { EnableProductlevelInvoiceGen = enableProductlevelInvoiceGen; }
        if (ProductLevelInvoiceGenPriorToDueDateInDays != productLevelInvoiceGenPriorToDueDateInDays) { ProductLevelInvoiceGenPriorToDueDateInDays = productLevelInvoiceGenPriorToDueDateInDays; }
        if (MaxCreditAmount != maxCreditAmount) { MaxCreditAmount = maxCreditAmount; }
        if (isActiveOrPendingProducts != IsActiveOrPendingProducts) { IsActiveOrPendingProducts = isActiveOrPendingProducts; }
        return this;
    }
}
