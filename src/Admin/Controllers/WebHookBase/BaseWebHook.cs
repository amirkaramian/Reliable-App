using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyReliableSite.Application.Scripting.Interfaces;
using System.Net;

namespace MyReliableSite.Admin.API.Controllers.WebHookBase;

public class BaseWebHook
{
}

/// <summary>
/// Base Class For All Handlers.
/// </summary>
[ApiController]
[Route("{prefix:webhookRoutePrefix}/[controller]")]
public abstract class ResponseHandler<TRequest, TResponse> : ControllerBase
{
    [HttpPost]
    [Route("")]
    public abstract Task<TResponse> Handle([FromBody] TRequest request);
}

[ApiController]
[Route("{prefix:webhookRoutePrefix}/[controller]")]
public abstract class AcceptedHandler<TRequest> : ControllerBase
{
    [HttpPost]
    [Route("")]
    [Status(HttpStatusCode.Accepted)]
    public abstract Task Handle([FromBody] TRequest request);
}

public class Status : ActionFilterAttribute
{
    private readonly HttpStatusCode _statusCode;

    public Status(HttpStatusCode statusCode)
    {
        this._statusCode = statusCode;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        context.Result = new StatusCodeResult((int)_statusCode);
    }
}

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly IScriptingService _scriptingService;

    public WebhookController(IScriptingService scriptingService)
    {
        _scriptingService = scriptingService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Handle()
    {
        string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        _scriptingService.HandleWebhook(json);
        return Accepted();
    }
}