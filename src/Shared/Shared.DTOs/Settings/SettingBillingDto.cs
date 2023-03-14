namespace MyReliableSite.Shared.DTOs.Settings;

public class SettingBillingDto : IDto
{
    public Guid Id { get; set; }
    public int MaxNumberOfRefunds { get; set; }
    public decimal MinOrderAmount { get; set; }
    public string Tenant { get; set; }
}