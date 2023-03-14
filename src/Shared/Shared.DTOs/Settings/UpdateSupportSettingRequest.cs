using MyReliableSite.Shared.DTOs.Storage;

namespace MyReliableSite.Shared.DTOs.Settings;

public class UpdateSupportSettingRequest : IMustBeValid
{
    public int MaxNumberOfSubCategories { get; set; }
    public bool AutoApproveNewArticles { get; set; }
    public string Tenant { get; set; }
}