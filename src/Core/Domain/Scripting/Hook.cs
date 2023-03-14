using MyReliableSite.Domain.Common.Contracts;
using MyReliableSite.Shared.DTOs.Scripting;

namespace MyReliableSite.Domain.Scripting;

public class Hook : AuditableEntity
{
    public string Name { get; set; }
    public HookType Type { get; set; }
    public string Script { get; set; }
    public string Module { get; set; }

    public Hook(string name, HookType type, string script, string module)
    {
        Name = name;
        Type = type;
        Script = script;
        Module = module;
    }

}

public class ServerHook : AuditableEntity
{
    public string Name { get; set; }
    public HookEvent ServerEvent { get; set; }
    public string Script { get; set; }

    public ServerHook(string name, HookEvent serverEvent, string script)
    {
        Name = name;
        ServerEvent = serverEvent;
        Script = script;
    }
}

public class PluginHook
{
    public string Trigger { get; set; }
    private readonly dynamic _hook;

    public PluginHook(dynamic hook)
    {
        _hook = hook;
        Trigger = hook.Trigger;
    }

    public async Task<object> Run(object data, object localApi)
    {
        return await _hook.Run(data, localApi);
    }
}