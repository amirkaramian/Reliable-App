namespace MyReliableSite.Application.Settings;

public class MiddlewareSettings
{
    public bool EnableHttpsLogging { get; set; } = false;
    public bool EnableLocalization { get; set; } = false;
    public bool APIKeysValidation { get; set; } = false;
    public string ModuleManageFileNameWithPath { get; set; }
    public string ModuleExcludedForMiddlewareValidation { get; set; }
}
