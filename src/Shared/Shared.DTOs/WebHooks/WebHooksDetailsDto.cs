using MyReliableSite.Shared.DTOs.ManageModule;

namespace MyReliableSite.Shared.DTOs.WebHooks;

public class WebHooksDetailsDto : IDto
{
    public Guid Id { get; set; }
    public string WebHookUrl { get; set; }
    public string ModuleId { get; set; }
    public ModuleDto Module { get; set; }

    public WebHookAction Action { get; set; }
    public bool IsActive { get; set; }

    public DateTime? LastTrigger { get; set; }
    public string Tenant { get; set; }
}

public enum WebHookAction
{
    Create,
    Update,
    Delete
}
