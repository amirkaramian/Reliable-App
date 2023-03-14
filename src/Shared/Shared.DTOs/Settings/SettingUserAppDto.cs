namespace MyReliableSite.Shared.DTOs.Settings;

public class SettingUserAppDto : IDto
{
    public Guid Id { get; set; }
    public bool ClientStatus { get; set; }
    public int RequestPerIPOverride { get; set; }
    public int IPRestrictionIntervalOverrideInSeconds { get; set; }
    public int APIKeyLimitOverride { get; set; }
    public int APIKeyIntervalOverrideInSeconds { get; set; }
    public string RestrictAccessToIPAddress { get; set; }
    public DateOnly ExtendSuspensionDate { get; set; }
    public string UserId { get; set; }
    public string Tenant { get; set; }
}