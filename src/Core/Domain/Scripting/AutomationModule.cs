using MyReliableSite.Domain.Common.Contracts;

namespace MyReliableSite.Domain.Scripting;

public class AutomationModule : AuditableEntity
{
    public string Name { get; set; }
    public string CustomFields { get; set; }
    public string Action { get; set; }
    public string MetaData { get; set; }
    public string Script { get; set; }

    public AutomationModule(string name, string customFields, string action, string metaData, string script)
    {
        Name = name;
        CustomFields = customFields;
        Action = action;
        MetaData = metaData;
        Script = script;
    }
}

public class Automation
{
    public string CustomFields { get; set; }
    private readonly dynamic _auto;
    public Automation(dynamic auto)
    {
        _auto = auto;
        CustomFields = auto.CustomFields;
    }

    public async Task<object> OnSuspend(object data, dynamic localApi)
    {
        return await _auto.OnSuspend(data, localApi);
    }

    public async Task<object> OnCreate(object data, dynamic localApi)
    {
        return await _auto.OnCreate(data, localApi);
    }

    public async Task<object> OnRenew(object data, dynamic localApi)
    {
        return await _auto.OnRenew(data, localApi);
    }

    public async Task<object> OnTerminate(object data, dynamic localApi)
    {
        return await _auto.OnTerminate(data, localApi);
    }
}
