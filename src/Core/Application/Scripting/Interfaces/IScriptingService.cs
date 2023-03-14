using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Products;
using MyReliableSite.Domain.Scripting;
using MyReliableSite.Shared.DTOs.Scripting;

namespace MyReliableSite.Application.Scripting.Interfaces;

public interface IScriptingService : ITransientService
{
    Task<Result<Guid>> CreateAutoModule(AddAutoModuleRequest request);
    Task<Result<Guid>> CreateHook(AddHookRequest request);
    Task<Result<Guid>> CreateServerHook(AddServerHookRequest request);
    Task<Result<Guid>> DeleteAutoModule(Guid id);
    Task<Result<Guid>> DeleteHook(Guid id);
    Task<Result<Guid>> DeleteServerHook(Guid id);
    Task<Result<Guid>> EditScriptAync(EditScriptRequest request);
    Task<Result<object>> ExecuteFromFileName(string name, object data = null);
    Result<object> ExecuteFromText(RunScriptRequest request);
    Task<Result<List<AutomationModuleDto>>> GetModulesAsync();
    Task<Result<List<HookDto>>> GetScriptsAsync();
    Task<Result<List<ServerHook>>> GetServerHooksAsync();
    void HandleWebhook(string json);
    Task ResolveHook(PluginHook hook, object data);
    Task<Result<object>> RunHooksAsync(RunHooksRequest request);
    Task RunModule(Guid id, Product product);
    Task RunProductHooks(Product product);
    Task<Result<Guid>> SaveScriptAync(SaveScriptRequest request);
    Result<object> Test();
    Task TriggerHooks(string trigger, object data);
    Task<Result<Guid>> UpdateAutoModule(EditAutoModuleRequest request);
    Task<Result<Guid>> UpdateHook(EditHookRequest request);
    Task<Result<Guid>> UpdateServerHook(EditServerHookRequest request);
}
