namespace MyReliableSite.Shared.DTOs.WebHooks;

public class UpdateWebHooksRequest : IMustBeValid
{
    public string WebHookUrl { get; set; }
    public string ModuleId { get; set; }

    public bool IsActive { get; set; }
    public WebHookAction Action { get; set; }
}
