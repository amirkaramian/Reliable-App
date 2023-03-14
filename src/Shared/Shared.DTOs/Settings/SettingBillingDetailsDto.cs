namespace MyReliableSite.Shared.DTOs.Settings;

public class SettingBillingDetailsDto : IDto
{
    public Guid Id { get; set; }
    public int MaxNumberOfRefunds { get; set; }
    public decimal MinOrderAmount { get; set; }
    public decimal RefundRetainPercentage { get; set; }
    public bool AutoInvoiceGeneration { get; set; }
    public int AutoInvoicePriorToDueDateInDays { get; set; }
    public bool EnableProductlevelInvoiceGen { get; set; }
    public int ProductLevelInvoiceGenPriorToDueDateInDays { get; set; }
    public string Tenant { get; set; }
    public decimal MaxCreditAmount { get; set; }
    public bool IsActiveOrPendingProducts { get; set; }
}