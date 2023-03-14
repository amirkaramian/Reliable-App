using System.Dynamic;
using System.Text.Json;

namespace MyReliableSite.Shared.DTOs.Scripting;

public class RunScriptRequest : IMustBeValid
{
    public string Script { get; set; }
    public object Data { get; set; } = null;
}

public class SaveScriptRequest : IMustBeValid
{
    public string Name { get; set; }
    public string Script { get; set; }
}

public class EditScriptRequest : IMustBeValid
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Script { get; set; }
}