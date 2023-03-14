using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Settings;

namespace MyReliableSite.Client.API.Controllers.Setting;
public class UserAppSettingsController : BaseController
{
    private readonly ISettingService _service;

    public UserAppSettingsController(ISettingService service)
    {
        _service = service;
    }

    /// <summary>
    /// retrive the User Setting against specific user id.
    /// </summary>
    /// <response code="200">User Setting returns.</response>
    /// <response code="400">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<SettingUserAppDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("GetUserAppSettingByUserId/{userid}")]
    [SwaggerHeader("tenant", "Settings", "View", "Input your tenant to access this API i.e. admin for test", "client", true, false)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetUserAppSettingByUserIdAsync(string userid)
    {
        var result = await _service.GetUserAppSettingByUserIdAsync(userid);
        return Ok(result);
    }

    /// <summary>
    /// update a specific User Setting by unique id.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("autobill/{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Settings", "Update", "Input your tenant to access this API i.e. admin for test", "client", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> UpdateAutoBillAsync(UpdateAutoBillUserAppSettingRequest request, Guid id)
    {
        return Ok(await _service.UpdateAutoBillUserAppSettingAsync(request, id));
    }
}
