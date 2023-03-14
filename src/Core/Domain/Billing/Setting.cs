using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;

namespace MyReliableSite.Domain.Billing;

public class Setting : AuditableEntity, IMustHaveTenant
{
    public string DateFormat { get; set; }
    public string DefaultCountry { get; set; }
    public int AutoRefreshInterval { get; set; }
    public string Tenant { get; set; }
    public bool LogRotation { get; set; }
    public int LogRotationDays { get; set; }
    public int RequestsIntervalPerIPAfterLimitAdminInSeconds { get; set; } = 60;
    public bool EnableAPIAccessAdmin { get; set; } = true;
    public bool EnableAPIAccessClient { get; set; } = true;
    public bool EnableClientRecaptcha { get; set; } = false;
    public bool EnableAdminRecaptcha { get; set; } = false;
    public int RequestsPerIPAdmin { get; set; } = 50;
    public int RequestsPerIPClient { get; set; } = 50;
    public int LoginRequestsPerIPAdmin { get; set; } = 50;
    public int LoginRequestsPerIPClient { get; set; } = 50;
    public int RequestsIntervalPerIPAfterLimitClientInSeconds { get; set; } = 60;

    public int DefaultInactivityMinutesLockAdmin { get; set; }
    public int DefaultInactivityMinutesLockClient { get; set; }
    public int TrustDeviceinDays { get; set; }
    public bool ForceAdminMFA { get; set; }
    public bool ForceClientMFA { get; set; }
    public bool EnableAdminMFA { get; set; }
    public bool EnableClientMFA { get; set; }

    public bool GoogleAuthenticator { get; set; }
    public bool MicrosoftAuthenticator { get; set; }
    public string Module1Settings { get; set; }
    public string Module2Settings { get; set; }

    public bool EnableThirdPartyAPIkeys { get; set; }
    public int NumberofRequestsPerIpApiKey { get; set; }
    public int IntervalBeforeNextAPIkeyRequestInSeconds { get; set; }
    public int LoginIntervalInSeconds_PortalSettings { get; set; }
    public bool EnableLoginIntervalInSeconds_PortalSettings { get; set; }
    public string BillPrefix { get; set; } = "#Invoice-";
    public int DefaultBillDueDays { get; set; } = 7;
    public decimal? VAT { get; set; } = 0;
    public string CompanyName { get; set; }
    public bool IsActiveOrPendingProducts { get; set; }
    public decimal MaxCreditAmount { get; set; }
    public Setting()
    {

    }

    public Setting(
        string dateFormat,
        string defaultCountry,
        int autoRefreshInterval,
        bool? logRotation,
        int logRotationDays,
        int requestsIntervalPerIPAfterLimitAdminInSeconds,
        bool? enableAPIAccessAdmin,
        bool? enableAPIAccessClient,
        int requestsPerIPAdmin,
        int requestsPerIPClient,
        int loginRequestsPerIPAdmin,
        int loginRequestsPerIPClient,
        int requestsIntervalPerIPAfterLimitClientInSeconds,
        int defaultInactivityMinutesLockAdmin,
        int defaultInactivityMinutesLockClient,
        bool? forceAdminMFA,
        bool? forceClientMFA,
        bool? enableAdminMFA,
        bool? enableClientMFA,
        int trustDeviceinDays,
        bool? googleAuthenticator,
        bool? microsoftAuthenticator,
        string module1Settings,
        string module2Settings,
        bool? enableThirdPartyAPIkeys,
        int numberofRequestsPerIpApiKey,
        int intervalBeforeNextAPIkeyRequestInSeconds,
        int loginIntervalInSeconds_PortalSettings,
        bool? enableLoginIntervalInSeconds_PortalSettings,
        string billPrefix,
        int defaultBillDueDays,
        decimal? vat,
        string companyName,
        bool? enableClientRecaptcha,
        bool? enableAdminRecaptcha,
        bool isActiveOrPendingProducts,
        decimal maxCreditAmount)
    {
        DateFormat = dateFormat;
        DefaultCountry = defaultCountry;
        AutoRefreshInterval = autoRefreshInterval;
        LogRotation = logRotation.HasValue ? logRotation.Value : false;
        LogRotationDays = logRotationDays;
        RequestsIntervalPerIPAfterLimitAdminInSeconds = requestsIntervalPerIPAfterLimitAdminInSeconds;
        EnableAPIAccessAdmin = enableAPIAccessAdmin.HasValue ? enableAPIAccessAdmin.Value : false;
        EnableAPIAccessClient = enableAPIAccessClient.HasValue ? enableAPIAccessClient.Value : false;
        RequestsPerIPAdmin = requestsPerIPAdmin;
        RequestsPerIPClient = requestsPerIPClient;
        LoginRequestsPerIPAdmin = loginRequestsPerIPAdmin;
        LoginRequestsPerIPClient = loginRequestsPerIPClient;
        RequestsIntervalPerIPAfterLimitClientInSeconds = requestsIntervalPerIPAfterLimitClientInSeconds;
        ForceAdminMFA = forceAdminMFA.HasValue ? forceAdminMFA.Value : false;
        ForceClientMFA = forceClientMFA.HasValue ? forceClientMFA.Value : false;
        EnableAdminMFA = enableAdminMFA.HasValue ? enableAdminMFA.Value : false;
        EnableClientMFA = enableClientMFA.HasValue ? enableClientMFA.Value : false;
        DefaultInactivityMinutesLockAdmin = defaultInactivityMinutesLockAdmin;
        DefaultInactivityMinutesLockClient = defaultInactivityMinutesLockClient;
        TrustDeviceinDays = trustDeviceinDays;
        GoogleAuthenticator = googleAuthenticator.HasValue ? googleAuthenticator.Value : false;
        MicrosoftAuthenticator = microsoftAuthenticator.HasValue ? microsoftAuthenticator.Value : false;
        Module1Settings = module1Settings;
        Module2Settings = module2Settings;
        EnableThirdPartyAPIkeys = enableThirdPartyAPIkeys.HasValue ? enableThirdPartyAPIkeys.Value : false;
        NumberofRequestsPerIpApiKey = numberofRequestsPerIpApiKey;
        IntervalBeforeNextAPIkeyRequestInSeconds = intervalBeforeNextAPIkeyRequestInSeconds;
        LoginIntervalInSeconds_PortalSettings = loginIntervalInSeconds_PortalSettings;
        EnableLoginIntervalInSeconds_PortalSettings = enableLoginIntervalInSeconds_PortalSettings.HasValue ? enableLoginIntervalInSeconds_PortalSettings.Value : false;
        BillPrefix = billPrefix;
        BillPrefix = billPrefix;
        DefaultBillDueDays = defaultBillDueDays;
        VAT = vat;
        CompanyName = companyName;
        EnableAdminRecaptcha = enableAdminRecaptcha.HasValue ? enableAdminRecaptcha.Value : false;
        EnableClientRecaptcha = enableClientRecaptcha.HasValue ? enableClientRecaptcha.Value : false;
        IsActiveOrPendingProducts = isActiveOrPendingProducts;
        MaxCreditAmount = maxCreditAmount;
    }

    public Setting Update(
        string dateFormat,
        string defaultCountry,
        int autoRefreshInterval,
        bool? logRotation,
        int logRotationDays,
        int requestsIntervalPerIPAfterLimitAdminInSeconds,
        bool? enableAPIAccessAdmin,
        bool? enableAPIAccessClient,
        int requestsPerIPAdmin,
        int requestsPerIPClient,
        int loginRequestsPerIPAdmin,
        int loginRequestsPerIPClient,
        int requestsIntervalPerIPAfterLimitClientInSeconds,
        int defaultInactivityMinutesLockAdmin,
        int defaultInactivityMinutesLockClient,
        bool? forceAdminMFA,
        bool? forceClientMFA,
        bool? enableAdminMFA,
        bool? enableClientMFA,
        int trustDeviceinDays,
        bool? googleAuthenticator,
        bool? microsoftAuthenticator,
        string module1Settings,
        string module2Settings,
        bool? enableThirdPartyAPIkeys,
        int numberofRequestsPerIpApiKey,
        int intervalBeforeNextAPIkeyRequestInSeconds,
        int loginIntervalInSeconds_PortalSettings,
        bool? enableLoginIntervalInSeconds_PortalSettings,
        string billPrefix,
        int defaultBillDueDays,
        decimal? vat,
        string companyName,
        bool? enableClientRecaptcha,
        bool? enableAdminRecaptcha,
        bool isActiveOrPendingProducts,
        decimal maxCreditAmount)
    {

        if (!string.IsNullOrWhiteSpace(dateFormat) && !string.Equals(DateFormat, dateFormat, StringComparison.InvariantCultureIgnoreCase)) DateFormat = dateFormat;
        if (!string.IsNullOrWhiteSpace(defaultCountry) && !string.Equals(DefaultCountry, defaultCountry, StringComparison.InvariantCultureIgnoreCase)) DefaultCountry = defaultCountry;
        if (autoRefreshInterval > 0 && AutoRefreshInterval != autoRefreshInterval) AutoRefreshInterval = autoRefreshInterval;
        if (logRotation.HasValue && LogRotation != logRotation) LogRotation = logRotation.Value;
        if (logRotationDays > 0 && LogRotationDays != logRotationDays) LogRotationDays = logRotationDays;
        if (requestsIntervalPerIPAfterLimitAdminInSeconds > 0 && RequestsIntervalPerIPAfterLimitAdminInSeconds != requestsIntervalPerIPAfterLimitAdminInSeconds) RequestsIntervalPerIPAfterLimitAdminInSeconds = requestsIntervalPerIPAfterLimitAdminInSeconds;
        if (enableAPIAccessAdmin.HasValue && EnableAPIAccessAdmin != enableAPIAccessAdmin) EnableAPIAccessAdmin = enableAPIAccessAdmin.Value;
        if (enableAPIAccessClient.HasValue && EnableAPIAccessClient != enableAPIAccessClient) EnableAPIAccessClient = enableAPIAccessClient.Value;
        if (requestsPerIPAdmin > 0 && RequestsPerIPAdmin != requestsPerIPAdmin) RequestsPerIPAdmin = requestsPerIPAdmin;
        if (requestsPerIPClient > 0 && RequestsPerIPClient != requestsPerIPClient) RequestsPerIPClient = requestsPerIPClient;
        if (loginRequestsPerIPAdmin > 0 && LoginRequestsPerIPAdmin != loginRequestsPerIPAdmin) LoginRequestsPerIPAdmin = loginRequestsPerIPAdmin;
        if (loginRequestsPerIPClient > 0 && LoginRequestsPerIPClient != loginRequestsPerIPClient) LoginRequestsPerIPClient = loginRequestsPerIPClient;
        if (requestsIntervalPerIPAfterLimitClientInSeconds > 0 && RequestsIntervalPerIPAfterLimitClientInSeconds != requestsIntervalPerIPAfterLimitClientInSeconds) RequestsIntervalPerIPAfterLimitClientInSeconds = requestsIntervalPerIPAfterLimitClientInSeconds;
        if (defaultInactivityMinutesLockClient > 0 && DefaultInactivityMinutesLockClient != defaultInactivityMinutesLockClient) DefaultInactivityMinutesLockClient = defaultInactivityMinutesLockClient;
        if (defaultInactivityMinutesLockAdmin > 0 && DefaultInactivityMinutesLockAdmin != defaultInactivityMinutesLockAdmin) DefaultInactivityMinutesLockAdmin = defaultInactivityMinutesLockAdmin;
        if (forceAdminMFA.HasValue && ForceAdminMFA != forceAdminMFA) ForceAdminMFA = forceAdminMFA.Value;
        if (forceClientMFA.HasValue && ForceClientMFA != forceClientMFA) ForceClientMFA = forceClientMFA.Value;
        if (enableAdminMFA.HasValue && EnableAdminMFA != enableAdminMFA) EnableAdminMFA = enableAdminMFA.Value;
        if (enableClientMFA.HasValue && EnableClientMFA != enableClientMFA) EnableClientMFA = enableClientMFA.Value;
        if (trustDeviceinDays > 0 && TrustDeviceinDays != trustDeviceinDays) TrustDeviceinDays = trustDeviceinDays;
        if (googleAuthenticator.HasValue && GoogleAuthenticator != googleAuthenticator) GoogleAuthenticator = googleAuthenticator.Value;
        if (microsoftAuthenticator.HasValue && MicrosoftAuthenticator != microsoftAuthenticator) MicrosoftAuthenticator = microsoftAuthenticator.Value;

        if (!string.IsNullOrWhiteSpace(module1Settings) && !string.Equals(Module1Settings, module1Settings, StringComparison.InvariantCultureIgnoreCase)) Module1Settings = module1Settings;
        if (!string.IsNullOrWhiteSpace(module2Settings) && !string.Equals(Module2Settings, module2Settings, StringComparison.InvariantCultureIgnoreCase)) Module2Settings = module2Settings;
        if (enableThirdPartyAPIkeys.HasValue && EnableThirdPartyAPIkeys != enableThirdPartyAPIkeys) EnableThirdPartyAPIkeys = enableThirdPartyAPIkeys.Value;
        if (numberofRequestsPerIpApiKey > 0 && NumberofRequestsPerIpApiKey != numberofRequestsPerIpApiKey) NumberofRequestsPerIpApiKey = numberofRequestsPerIpApiKey;
        if (intervalBeforeNextAPIkeyRequestInSeconds > 0 && IntervalBeforeNextAPIkeyRequestInSeconds != intervalBeforeNextAPIkeyRequestInSeconds) IntervalBeforeNextAPIkeyRequestInSeconds = intervalBeforeNextAPIkeyRequestInSeconds;
        if (loginIntervalInSeconds_PortalSettings > 0 && LoginIntervalInSeconds_PortalSettings != loginIntervalInSeconds_PortalSettings) LoginIntervalInSeconds_PortalSettings = loginIntervalInSeconds_PortalSettings;
        if (enableLoginIntervalInSeconds_PortalSettings.HasValue && EnableLoginIntervalInSeconds_PortalSettings != enableLoginIntervalInSeconds_PortalSettings) EnableLoginIntervalInSeconds_PortalSettings = enableLoginIntervalInSeconds_PortalSettings.Value;
        if (!string.IsNullOrWhiteSpace(billPrefix) && !string.Equals(BillPrefix, billPrefix, StringComparison.InvariantCultureIgnoreCase)) BillPrefix = billPrefix;
        if (defaultBillDueDays > 0 && DefaultBillDueDays != defaultBillDueDays) DefaultBillDueDays = defaultBillDueDays;
        if (VAT.HasValue && vat > 0 && VAT != vat) VAT = vat;
        if (enableClientRecaptcha.HasValue && EnableClientRecaptcha != enableClientRecaptcha) EnableClientRecaptcha = enableClientRecaptcha.Value;
        if (enableAdminRecaptcha.HasValue && EnableAdminRecaptcha != enableAdminRecaptcha) EnableAdminRecaptcha = enableAdminRecaptcha.Value;

        if (!string.IsNullOrWhiteSpace(companyName) && !string.Equals(CompanyName, companyName, StringComparison.InvariantCultureIgnoreCase)) CompanyName = companyName;
        if(IsActiveOrPendingProducts != isActiveOrPendingProducts) IsActiveOrPendingProducts = isActiveOrPendingProducts;
        if(MaxCreditAmount != maxCreditAmount) MaxCreditAmount = maxCreditAmount;
        return this;
    }
}