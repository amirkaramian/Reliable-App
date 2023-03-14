namespace MyReliableSite.Shared.DTOs.Settings;

public class CreateSupportSettingRequest : IMustBeValid
{
    public int MaxNumberOfSubCategories { get; set; }
    public bool AutoApproveNewArticles { get; set; }
    public string Tenant { get; set; }
}
