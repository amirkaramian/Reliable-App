namespace MyReliableSite.Shared.DTOs.Settings;

public class SettingSupportDto : IDto
{
    public Guid Id { get; set; }

    public string DateFormat { get; set; }
    public string DefaultCountry { get; set; }
    public string TermsOfServiceURL { get; set; }
    public bool TermsOfServiceAgreement { get; set; } = true;
    public int RecordsToDisplay { get; set; }
    public int AutoRefreshInterval { get; set; }
    public string Tenant { get; set; }
    public bool LogRotation { get; set; }
    public int LogRotationDays { get; set; }
    public int RequestsIntervalPerIPAfterLimitAdminInSeconds { get; set; }
    public bool EnableAPIAccessAdmin { get; set; }
    public bool EnableAPIAccessClient { get; set; }
    public int RequestsPerIPAdmin { get; set; }
    public int RequestsPerIPClient { get; set; }
    public int LoginRequestsPerIPAdmin { get; set; }
    public int LoginRequestsPerIPClient { get; set; }
    public int defaultInactivityMinutesLockAdmin { get; set; }
    public int defaultInactivityMinutesLockClient { get; set; }
    public int RequestsIntervalPerIPAfterLimitClientInSeconds { get; set; }
    public int TrustDeviceinDays { get; set; }
    public bool ForceMFA { get; set; }
    public int MaxNumberOfRefunds { get; set; }
    public decimal MinOrderAmount { get; set; }
    public bool GoogleAuthenticator { get; set; }
    public bool MicrosoftAuthenticator { get; set; }
    public string Module1Settings { get; set; }
    public string Module2Settings { get; set; }
    public bool EnableThirdPartyAPIkeys { get; set; }
    public int NumberofRequestsPerIpApiKey { get; set; }
    public int IntervalBeforeNextAPIkeyRequestInSeconds { get; set; }
    public int LoginIntervalInSeconds_PortalSettings { get; set; }
}