using MyReliableSite.Shared.DTOs.Products;

namespace MyReliableSite.Shared.DTOs.Scripting;

public class AddHookRequest
{
    public string Name { get; set; }
    public HookType Type { get; set; }
    public string Script { get; set; }
    public string Module { get; set; }
}

public class EditHookRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public HookType Type { get; set; }
    public string Script { get; set; }
    public string Module { get; set; }
}

public class AddAutoModuleRequest
{
    public string Name { get; set; }
    public string CustomFields { get; set; }
    public string Action { get; set; } = string.Empty;
    public string MetaData { get; set; } = string.Empty;
    public string Script { get; set; }
}

public class EditAutoModuleRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CustomFields { get; set; }
    public string Action { get; set; } = string.Empty;
    public string MetaData { get; set; } = string.Empty;
    public string Script { get; set; }
}

public class AddServerHookRequest
{
    public string Name { get; set; }
    public HookEvent ServerEvent { get; set; }
    public string Script { get; set; }
}

public class EditServerHookRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public HookEvent ServerEvent { get; set; }
    public string Script { get; set; }
}

public class ProductModuleRequest
{
    public string Module { get; set; }
    public AutoSetup ProductSetup { get; set; }
    public object ExtraData { get; set; }
}

public class RunHooksRequest
{
    public Guid ProductId { get; set; }
    public HookType HookType { get; set; }
}

public class RunHooksResult
{
    public bool Success { get; set; }
    public object Data { get; set; }

    public RunHooksResult(bool success, object data = null)
    {
        Success = success;
        Data = data;
    }
}

public enum HookType
{
    Create = 0,
    Delete = 1,
    Suspend = 2,
    Terminate = 3,
    Cancel = 4,
    Renew = 5,
}

public enum HookEvent
{
    ProductCreated = 0,
    ProductUpdated = 1,
    ProductDeleted = 2,
    OrderCreated = 3,
    OrderUpdated = 4,
    OrderDeleted = 5,
}