using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Settings;

namespace MyReliableSite.Client.API.Controllers.Setting;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : BaseController
{
    private readonly ISettingService _service;

    public SettingsController(ISettingService service)
    {
        _service = service;
    }

    /// <summary>
    /// retrive the Setting against specific tenant.
    /// </summary>
    /// <response code="200">Setting returns.</response>
    /// <response code="400">Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{tenant}")]
    [ProducesResponseType(typeof(Result<SettingDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getsettingswithtenant/{tenant}")]
    [SwaggerHeader("tenant", "Settings", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetSettingsWithTenantAsync(string tenant)
    {
        var result = await _service.GetSettingDetailsAsync(tenant);
        return Ok(result);
    }
}
