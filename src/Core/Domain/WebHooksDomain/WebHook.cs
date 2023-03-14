using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Domain.Contracts;
using MyReliableSite.Domain.ManageModule;
using MyReliableSite.Shared.DTOs.WebHooks;

namespace MyReliableSite.Domain.WebHooksDomain;

// DTO
public class WebHook : AuditableEntity, IMustHaveTenant
{
    public WebHook(string webHookUrl, string moduleId, WebHookAction action, bool isActive)
    {
        WebHookUrl = webHookUrl;
        ModuleId = moduleId;
        Action = action;
        IsActive = isActive;
    }

    public WebHook Update(string webHookUrl, string moduleId, WebHookAction action, bool isActive)
    {
        if (webHookUrl != WebHookUrl) { WebHookUrl = webHookUrl; }
        if (!string.IsNullOrWhiteSpace(moduleId) && !string.Equals(ModuleId, moduleId, StringComparison.InvariantCultureIgnoreCase)) ModuleId = moduleId;
        if (action != Action) Action = action;
        if (isActive != IsActive) { IsActive = isActive; }
        return this;
    }

    public string WebHookUrl { get; set; }
    public bool IsActive { get; set; }
    public string Tenant { get; set; }
    public string ModuleId { get; set; }
    public virtual Module Module { get; set; }
    public WebHookAction Action { get; set; }
    public DateTime? LastTrigger { get; set; }
}
