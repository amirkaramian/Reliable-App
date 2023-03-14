namespace MyReliableSite.Shared.DTOs.Settings;

public class SettingSupportDetailsDto : IDto
{
    public Guid Id { get; set; }
    public int MaxNumberOfSubCategories { get; set; }
    public bool AutoApproveNewArticles { get; set; }
    public decimal RefundRetainPercentage { get; set; }
    public string Tenant { get; set; }
}