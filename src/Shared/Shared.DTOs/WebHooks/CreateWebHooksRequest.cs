namespace MyReliableSite.Shared.DTOs.WebHooks;

public class CreateWebHooksRequest : IMustBeValid
{
    public string WebHookUrl { get; set; }
    public bool IsActive { get; set; }
    public string ModuleId { get; set; }
    public WebHookAction Action { get; set; }
}