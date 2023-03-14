using System.Dynamic;

namespace MyReliableSite.Shared.DTOs.Scripting;

public class ScriptDto : IDto
{
    public string FileName { get; set; }
    public string Base64Data { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class HookDto : IDto
{
    public string Name { get; set; }
    public HookType Type { get; set; }
    public string Script { get; set; }
    public string Module { get; set; }
    public Guid Id { get; set; }
    public DateTime CreatedOn { get; set; }
}

public class AutomationModuleDto : IDto
{
    public string Name { get; set; }
    public ExpandoObject Fields { get; set; }
}
