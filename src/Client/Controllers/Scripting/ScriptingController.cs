using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Scripting.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Scripting;

namespace MyReliableSite.Client.API.Controllers.Scripting;
public class ScriptingController : BaseController
{
    private readonly IScriptingService _scriptingService;

    public ScriptingController(IScriptingService scriptingService)
    {
        _scriptingService = scriptingService;
    }

    [HttpPost("runhooks")]
    [ProducesResponseType(typeof(Result<List<RunHooksResult>>), 200)]
    [SwaggerHeader("tenant", "Scripting", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> RunHooksAsync(RunHooksRequest request)
    {
        return Ok(await _scriptingService.RunHooksAsync(request));
    }

}
