using IronPython.Hosting;
using IronPython.Runtime;
using Mapster;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Microsoft.Scripting.Runtime;
using MyReliableSite.Application.Common.Interfaces;
using MyReliableSite.Application.Exceptions;
using MyReliableSite.Application.Scripting.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.KnowledgeBase;
using MyReliableSite.Shared.DTOs.Scripting;
using System.Dynamic;
using Newtonsoft.Json;
using MyReliableSite.Domain.Scripting;
using MyReliableSite.Domain.Products;
using MyReliableSite.Shared.DTOs.Products;
using System.Runtime.Loader;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace MyReliableSite.Application.Scripting.Services;

public class ScriptingService : IScriptingService
{
    private const string _handlerScriptName = "handler";
    private const string AutomationsFolder = "Automations";
    private readonly string _hooksFolder;
    private readonly ScriptEngine _engine;
    private readonly IRepositoryAsync _repository;
    private readonly ILocalApi _localApi;
    private readonly IJobService _jobService;
    private readonly ILogger<ScriptingService> _logger;
    private readonly string _pyExtention;
    public ScriptingService(IRepositoryAsync repository, ILocalApi localApi, IJobService jobService, ILogger<ScriptingService> logger)
    {
        _engine = Python.CreateEngine();
        _repository = repository;
        _localApi = localApi;
        _pyExtention = ".py";
        _jobService = jobService;
        _logger = logger;
        _hooksFolder = Path.Combine(Directory.GetCurrentDirectory(), "Files", "Hooks");
    }

    public Result<object> ExecuteFromText(RunScriptRequest request)
    {
        ScriptScope scope = _engine.CreateScope();
        if(request.Data != null)
        {
            PythonDictionary element = JsonConvert.DeserializeObject<PythonDictionary>(request.Data.ToString());
            scope.SetVariable("data", element);
        }

        byte[] strCode = Convert.FromBase64String(request.Script);
        string code = System.Text.Encoding.UTF8.GetString(strCode);
        var source = _engine.CreateScriptSourceFromString(code);
        var data = source.Execute(scope);
        bool hasCommands = scope.TryGetVariable("commands", out PythonList commands);
        List<object> results = new List<object>();
        if (hasCommands)
        {
            foreach (dynamic command in commands)
            {
                Type type = typeof(ILocalApi);
                string c = command["command"] as string;
                PythonList pyList = command["param"] as PythonList;
                object[] param = pyList.ToArray();
                var com = type.GetMethod(c);
                object result = com.Invoke(_localApi, param);
                if (result != null) results.Add(result);
            }
        }

        dynamic finalResult = new ExpandoObject();
        finalResult.d = data;
        finalResult.cr = results;
        return Result<dynamic>.Success(data: finalResult);
    }

    public async Task<Result<object>> ExecuteFromFileName(string name, object data = null)
    {
        ServerFile file = await _repository.FirstByConditionAsync<ServerFile>(x => x.FileName.ToLower() == name.ToLower());
        if (file == null) throw new EntityNotFoundException("Script does not exist");
        string code = file.Base64Data;
        RunScriptRequest request = new() { Script = code, Data = data };
        return ExecuteFromText(request);
    }

    public async Task<Result<Guid>> SaveScriptAync(SaveScriptRequest request)
    {
        ServerFile file = new ServerFile(request.Name, _pyExtention, request.Script);
        var guid = await _repository.CreateAsync(file);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(guid);
    }

    public async Task<Result<List<HookDto>>> GetScriptsAsync()
    {
        var results = await _repository.GetListAsync<Hook>(x => x.DeletedOn == null);
        var finalResult = results.Adapt<List<HookDto>>();
        return await Result<List<HookDto>>.SuccessAsync(finalResult);
    }

    public async Task<Result<List<ServerHook>>> GetServerHooksAsync()
    {
        var results = await _repository.GetListAsync<ServerHook>(x => x.DeletedOn == null);
        return await Result<List<ServerHook>>.SuccessAsync(results);
    }

    public async Task<Result<List<AutomationModuleDto>>> GetModulesAsync()
    {
        var finalResult = new List<AutomationModuleDto>();
        string path = Directory.GetCurrentDirectory();
        path = Path.Combine(path, "Files", AutomationsFolder);

        foreach (string filePath in Directory.GetFiles(path))
        {
            try
            {
                AutomationModuleDto moduleDto = new AutomationModuleDto
                {
                    Name = Path.GetFileNameWithoutExtension(filePath)
                };
                Automation automation = GetAuto(moduleDto.Name);
                moduleDto.Fields = JsonConvert.DeserializeObject<ExpandoObject>(automation.CustomFields);
                finalResult.Add(moduleDto);

            }
            catch (Exception)
            {
            }
        }

        return await Result<List<AutomationModuleDto>>.SuccessAsync(finalResult);
    }

    public void HandleWebhook(string json)
    {
        object data = JsonConvert.DeserializeObject(json);
        _jobService.Enqueue(() => ExecuteFromFileName(_handlerScriptName, data));
    }

    public async Task<Result<Guid>> EditScriptAync(EditScriptRequest request)
    {
        ServerFile file = await _repository.GetByIdAsync<ServerFile>(request.Id);
        if (file == null) throw new EntityNotFoundException("Script does not exist");
        file.FileName = request.Name;
        file.Base64Data = request.Script;
        file.LastModifiedOn = DateTime.UtcNow;
        await _repository.UpdateAsync(file);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(request.Id);
    }

    public async Task<Result<Guid>> CreateHook(AddHookRequest request)
    {
        bool exits = await _repository.ExistsAsync<Hook>(x => x.Name == request.Name);
        if (exits) throw new EntityAlreadyExistsException("name already taken");
        Hook hook = new(request.Name, request.Type, request.Script, request.Module);
        Guid id = await _repository.CreateAsync(hook);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateHook(EditHookRequest request)
    {
        Hook hook = await _repository.GetByIdAsync<Hook>(request.Id);
        hook.Module = request.Module;
        hook.Script = request.Script;
        hook.Type = request.Type;
        hook.Name = request.Name;
        await _repository.UpdateAsync(hook);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(request.Id);
    }

    public async Task<Result<Guid>> DeleteHook(Guid id)
    {
        await _repository.RemoveByIdAsync<Hook>(id);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> CreateServerHook(AddServerHookRequest request)
    {
        ServerHook hook = new(request.Name, request.ServerEvent, request.Script);
        bool exits = await _repository.ExistsAsync<ServerHook>(x => x.Name == request.Name);
        if (exits) throw new EntityAlreadyExistsException("name already taken");
        Guid id = await _repository.CreateAsync(hook);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateServerHook(EditServerHookRequest request)
    {
        ServerHook hook = await _repository.GetByIdAsync<ServerHook>(request.Id);
        hook.ServerEvent = request.ServerEvent;
        hook.Script = request.Script;
        hook.Name = request.Name;
        await _repository.UpdateAsync(hook);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(request.Id);
    }

    public async Task<Result<Guid>> DeleteServerHook(Guid id)
    {
        await _repository.RemoveByIdAsync<ServerHook>(id);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> CreateAutoModule(AddAutoModuleRequest request)
    {
        AutomationModule module = new(request.Name, request.CustomFields, request.Action, request.MetaData, request.Script);
        bool exits = await _repository.ExistsAsync<AutomationModule>(x => x.Name == request.Name);
        if (exits) throw new EntityAlreadyExistsException("name already taken");
        Guid id = await _repository.CreateAsync(module);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> UpdateAutoModule(EditAutoModuleRequest request)
    {
        AutomationModule hook = await _repository.GetByIdAsync<AutomationModule>(request.Id);
        hook.MetaData = request.MetaData;
        hook.Script = request.Script;
        hook.Name = request.Name;
        hook.Action = request.Action;
        hook.CustomFields = request.CustomFields;
        await _repository.UpdateAsync(hook);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(request.Id);
    }

    public async Task<Result<Guid>> DeleteAutoModule(Guid id)
    {
        await _repository.RemoveByIdAsync<AutomationModule>(id);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task RunModule(Guid id, Product product)
    {
        var module = await _repository.GetByIdAsync<AutomationModule>(id);
        if (module == null) throw new EntityNotFoundException("Module does not exist");
        var details = product.Adapt<ProductDetailDto>();
        details.FieldData = JsonConvert.DeserializeObject<ExpandoObject>(product.ExtraData);
        object data = details;
        try
        {
            RunScriptRequest request = new() { Script = module.Script, Data = data };
            ExecuteFromText(request);
        }
        catch (Exception)
        {
        }

        var hooks = await _repository.FindByConditionAsync<Hook>(x => x.Module.ToLower() == module.Action.ToLower());
        foreach (Hook hook in hooks)
        {
            try
            {
                RunScriptRequest request = new() { Script = hook.Script, Data = data };
                ExecuteFromText(request);
            }
            catch (Exception)
            {
            }
        }
    }

    public async Task RunProductHooks(Product product)
    {
        var module = await _repository.GetByIdAsync<AutomationModule>(product.Id);
        if (module == null) throw new EntityNotFoundException("Module does not exist");
        var details = product.Adapt<ProductDetailDto>();
        details.FieldData = JsonConvert.DeserializeObject<ExpandoObject>(product.ExtraData);
        object data = details;
        HookType? hookType = null;
        switch (product.Status)
        {
            case ProductStatus.Pending:
            case ProductStatus.Active:
                break;
            case ProductStatus.Cancelled:
                hookType = HookType.Cancel;
                break;
            case ProductStatus.Suspended:
                hookType = HookType.Terminate;
                break;
        }

        if (hookType == null) return;
        var hooks = await _repository.FindByConditionAsync<Hook>(x => x.Module.ToLower() == module.Action.ToLower() && x.Type == hookType);
        foreach (Hook hook in hooks)
        {
            try
            {
                RunScriptRequest request = new() { Script = hook.Script, Data = data };
                ExecuteFromText(request);
            }
            catch (Exception)
            {
            }
        }
    }

    public async Task<Result<object>> RunHooksAsync(RunHooksRequest request)
    {
        var product = await _repository.GetByIdAsync<Product>(request.ProductId);
        if (product == null) throw new EntityNotFoundException("Product does not exist");
        if(string.IsNullOrEmpty(product.ModuleName)) throw new EntityNotFoundException("Product does not have a module");
        string filename = product.ModuleName + ".dll";
        string path = Directory.GetCurrentDirectory();
        path = Path.Combine(path, "Files", AutomationsFolder, filename);

        AssemblyLoadContext context = new(path);
        Assembly assembly = context.LoadFromAssemblyPath(path);
        var types = assembly.GetTypes();
        Type t = types[0];
        dynamic auto = Activator.CreateInstance(t);
        Automation automation = new(auto);
        object result = new ExpandoObject();
        try
        {
            switch (request.HookType)
            {
                case HookType.Create:
                    result = await automation.OnCreate(product, _localApi);
                    break;
                case HookType.Terminate:
                    result = await automation.OnTerminate(product, _localApi);
                    break;
                case HookType.Suspend:
                case HookType.Cancel:
                    result = await automation.OnSuspend(product, _localApi);
                    break;
                case HookType.Renew:
                    result = await automation.OnRenew(product, _localApi);
                    break;
                case HookType.Delete:
                    break;
            }

            _logger.LogInformation("{event} Triggered by {type} automation", t.Name, request.HookType.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError(e, "error running {event} Triggered by {type} automation", t.Name, request.HookType.ToString());
        }

        return await Result<object>.SuccessAsync(result);
    }

    public async Task TriggerHooks(string trigger, object data)
    {
        var hooks = GetAllHooks();
        foreach (var hook in hooks.Where(x => x.Trigger.ToLower() == trigger.ToLower()))
        {
            await ResolveHook(hook, data);
        }

    }

    public Result<object> Test()
    {
        const string code = "'Hello World!'";
        var source = _engine.CreateScriptSourceFromString(code);
        var data = source.Execute();
        return Result<object>.Success(data: data);
    }

    public async Task ResolveHook(PluginHook hook, object data)
    {
        try
        {
            object result = await hook.Run(data, _localApi);
            _logger.LogInformation("hook result {result}", result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "error running {trigger}", hook.Trigger);
        }
    }

    private Automation GetAuto(string name)
    {
        string filename = name + ".dll";
        string path = Directory.GetCurrentDirectory();
        path = Path.Combine(path, "Files", AutomationsFolder, filename);
        AssemblyLoadContext context = new(path);
        Assembly assembly = context.LoadFromAssemblyPath(path);
        var types = assembly.GetTypes();
        Type t = types[0];
        dynamic auto = Activator.CreateInstance(t);
        return (Automation)new(auto);
    }

    private List<PluginHook> GetAllHooks()
    {
        List<PluginHook> hooks = new();
        foreach(string filePath in Directory.GetFiles(_hooksFolder, "*.dll"))
        {
            try
            {
                AssemblyLoadContext context = new(filePath);
                Assembly assembly = context.LoadFromAssemblyPath(filePath);
                var types = assembly.GetTypes();
                Type t = types[0];
                dynamic hook = Activator.CreateInstance(t);
                hooks.Add((PluginHook)new(hook));
            }
            catch (Exception)
            {
            }
        }

        return hooks;
    }

    private void CheckDir()
    {
        string path = Directory.GetCurrentDirectory();
        path = Path.Combine(path, "Files", AutomationsFolder);
        if(!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    private void CheckHookDir()
    {
        string path = _hooksFolder;
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
}
