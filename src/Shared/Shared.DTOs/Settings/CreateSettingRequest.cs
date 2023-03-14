namespace MyReliableSite.Shared.DTOs.Settings;

public class CreateSettingRequest : IMustBeValid
{
    public string DateFormat { get; set; }
    public string DefaultCountry { get; set; }
    public int AutoRefreshInterval { get; set; }
    public string Tenant { get; set; }
    public bool? LogRotation { get; set; }
    public int LogRotationDays { get; set; }
    public int RequestsIntervalPerIPAfterLimitAdminInSeconds { get; set; }
    public bool? EnableAPIAccessAdmin { get; set; }
    public bool? EnableAPIAccessClient { get; set; }
    public int RequestsPerIPAdmin { get; set; }
    public int RequestsPerIPClient { get; set; }
    public int LoginRequestsPerIPAdmin { get; set; }
    public int LoginRequestsPerIPClient { get; set; }
    public int defaultInactivityMinutesLockAdmin { get; set; }
    public int defaultInactivityMinutesLockClient { get; set; }
    public int RequestsIntervalPerIPAfterLimitClientInSeconds { get; set; }
    public int TrustDeviceinDays { get; set; }
    public bool? ForceAdminMFA { get; set; }
    public bool? ForceClientMFA { get; set; }
    public bool? EnableClientRecaptcha { get; set; }
    public bool? EnableAdminRecaptcha { get; set; }
    public bool? GoogleAuthenticator { get; set; }
    public bool? MicrosoftAuthenticator { get; set; }
    public string Module1Settings { get; set; }
    public string Module2Settings { get; set; }
    public bool? EnableThirdPartyAPIkeys { get; set; }
    public int NumberofRequestsPerIpApiKey { get; set; }
    public int IntervalBeforeNextAPIkeyRequestInSeconds { get; set; }
    public int LoginIntervalInSeconds_PortalSettings { get; set; }
    public bool? EnableLoginIntervalInSeconds_PortalSettings { get; set; }
    public string BillPrefix { get; set; }
    public int DefaultBillDueDays { get; set; }
    public decimal? VAT { get; set; } = 0;
    public string CompanyName { get; set; }
    public bool? EnableAdminMFA { get; set; }
    public bool? EnableClientMFA { get; set; }
    public bool IsActiveOrPendingProducts { get; set; }
    public decimal MaxCreditAmount { get; set; }
}
