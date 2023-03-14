using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.WebHooks.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.WebHooks;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.WebHooks;

public class WebHooksController : BaseController
{
    private readonly IWebHooksService _service;

    public WebHooksController(IWebHooksService service)
    {
        _service = service;
    }

    [HttpPost("search")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.WebHooks.Search)]
    [SwaggerOperation(Summary = "Search WebHooks using available Filters.")]
    public async Task<IActionResult> SearchAsync(WebHookListFilter filter)
    {
        return Ok(await _service.SearchWebHookAsync(filter));
    }

    [HttpPost]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.WebHooks.Create)]
    public async Task<IActionResult> CreateAsync(CreateWebHooksRequest request)
    {
        return Ok(await _service.CreateWebHooksAsync(request));
    }

    [HttpPut("{id}")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.WebHooks.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateWebHooksRequest request, Guid id)
    {
        return Ok(await _service.UpdateWebHooksAsync(request, id));
    }

    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.WebHooks.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetWebHooksDetailsAsync(id);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.WebHooks.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var hookId = await _service.DeleteWebHooksAsync(id);
        return Ok(hookId);
    }
}
