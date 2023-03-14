using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Scripting.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Scripting;

namespace MyReliableSite.Admin.API.Controllers.Scripting;
public class ScriptingController : BaseController
{
    private readonly IScriptingService _scriptingService;

    public ScriptingController(IScriptingService scriptingService)
    {
        _scriptingService = scriptingService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<List<HookDto>>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetHooksAsync()
    {
        return Ok(await _scriptingService.GetScriptsAsync());
    }

    [HttpGet("automodule")]
    [ProducesResponseType(typeof(Result<List<AutomationModuleDto>>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetModulesAsync()
    {
        return Ok(await _scriptingService.GetModulesAsync());
    }

    [HttpGet("serverhook")]
    [ProducesResponseType(typeof(Result<List<AutomationModuleDto>>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetServerHooksAsync()
    {
        return Ok(await _scriptingService.GetServerHooksAsync());
    }

    [HttpPost]
    [ProducesResponseType(typeof(Result<object>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public IActionResult RunScript(RunScriptRequest request)
    {
        return Ok(_scriptingService.ExecuteFromText(request));
    }

    [HttpPut]
    [ProducesResponseType(typeof(Result<object>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SaveScriptAsync(SaveScriptRequest request)
    {
        return Ok(await _scriptingService.SaveScriptAync(request));
    }

    [HttpPut("hook")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> AddHooksAsync(AddHookRequest request)
    {
        return Ok(await _scriptingService.CreateHook(request));
    }

    [HttpPost("hook")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> EditHooksAsync(EditHookRequest request)
    {
        return Ok(await _scriptingService.UpdateHook(request));
    }

    [HttpDelete("hook/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> RemoveHooksAsync(Guid id)
    {
        return Ok(await _scriptingService.DeleteHook(id));
    }

    [HttpPut("serverhook")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> AddServerHooksAsync(AddServerHookRequest request)
    {
        return Ok(await _scriptingService.CreateServerHook(request));
    }

    [HttpPost("serverhook")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> EditServerHooksAsync(EditServerHookRequest request)
    {
        return Ok(await _scriptingService.UpdateServerHook(request));
    }

    [HttpDelete("serverhook/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> RemoveServerHooksAsync(Guid id)
    {
        return Ok(await _scriptingService.DeleteServerHook(id));
    }

    [HttpPut("automodule")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> AddModuleAsync(AddAutoModuleRequest request)
    {
        return Ok(await _scriptingService.CreateAutoModule(request));
    }

    [HttpPost("automodule")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> EditModulesAsync(EditAutoModuleRequest request)
    {
        return Ok(await _scriptingService.UpdateAutoModule(request));
    }

    [HttpDelete("automodule/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> RemoveAutoModuleAsync(Guid id)
    {
        return Ok(await _scriptingService.DeleteAutoModule(id));
    }

    [HttpPost("runhooks")]
    [ProducesResponseType(typeof(Result<List<RunHooksResult>>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> RunHooksAsync(RunHooksRequest request)
    {
        return Ok(await _scriptingService.RunHooksAsync(request));
    }

}
