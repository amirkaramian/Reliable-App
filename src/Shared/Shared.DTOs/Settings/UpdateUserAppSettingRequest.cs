namespace MyReliableSite.Shared.DTOs.Settings;

public class UpdateUserAppSettingRequest : IMustBeValid
{
    public bool ClientStatus { get; set; }
    public int RequestPerIPOverride { get; set; }
    public int IPRestrictionIntervalOverrideInSeconds { get; set; }
    public int APIKeyLimitOverride { get; set; }
    public int APIKeyIntervalOverrideInSeconds { get; set; }
    public List<string> RestrictAccessToIPAddress { get; set; }
    public DateTime ExtendSuspensionDate { get; set; }
    public string UserId { get; set; }
    public string Tenant { get; set; }
    public bool? AutoBill { get; set; }
    public bool IsActiveOrPendingProduct { get; set; }
    public decimal MaxCreditAmount { get; set; }
    public bool CanTakeOrders { get; set; }
    public bool AvallableForOrder { get; set; }
    public bool AutoAssignOrders { get; set; }
}