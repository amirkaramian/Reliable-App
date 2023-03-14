using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Settings.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Settings;
using Org.BouncyCastle.Asn1.Ocsp;

namespace MyReliableSite.Admin.API.Controllers.Setting;

public class UserAppSettingsController : BaseController
{
    private readonly ISettingService _service;

    public UserAppSettingsController(ISettingService service)
    {
        _service = service;
    }

    /// <summary>
    /// Create an User Setting.
    /// </summary>
    /// <response code="200">User Setting created.</response>
    /// <response code="400">User Setting already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Register", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Register)]
    public async Task<IActionResult> CreateAsync(CreateUserAppSettingRequest request)
    {
        return Ok(await _service.CreateUserAppSettingAsync(request));
    }

    /// <summary>
    /// update a specific User Setting by unique id.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> UpdateAsync(UpdateUserAppSettingRequest request, Guid id)
    {
        return Ok(await _service.UpdateUserAppSettingAsync(request, id));
    }

    /// <summary>
    /// retrive the User Setting against specific id.
    /// </summary>
    /// <response code="200">User Setting returns.</response>
    /// <response code="400">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<SettingUserAppDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetUserAppSettingDetailsAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// retrive the User Setting against specific tenant.
    /// </summary>
    /// <response code="200">User Setting returns.</response>
    /// <response code="400">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<SettingUserAppDetailsDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getsettingswithtenant/{tenant}")]
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetUserAppSettingsWithTenantAsync(string tenant)
    {
        var result = await _service.GetUserAppSettingDetailsAsync(tenant);
        return Ok(result);
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
    [SwaggerHeader("tenant", "Setting", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.View)]
    public async Task<IActionResult> GetUserAppSettingByUserIdAsync(string userid)
    {
        var result = await _service.GetUserAppSettingByUserIdAsync(userid);
        return Ok(result);
    }

    /// <summary>
    /// update Can Take Orders.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("enabletakeorders/{userid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> EnableTakeOrdersAsync(Guid userid)
    {
        return Ok(await _service.UpdateCantakeOrdersAsync(userid, true));
    }

    /// <summary>
    /// update Can Take Orders.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("disabletakeorders/{userid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> DisableTakeOrdersAsync(Guid userid)
    {
        return Ok(await _service.UpdateCantakeOrdersAsync(userid, false));
    }

    /// <summary>
    /// update Auto assign Orders.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("enableautoassignorders/{userid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> AutoAssignOrdersAsync(Guid userid)
    {
        return Ok(await _service.UpdateAutoAssignOrdersAsync(userid, true));
    }

    /// <summary>
    /// update Auto assign Orders.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("disableautoassignorders/{userid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> DisableAutoAssignOrdersAsync(Guid userid)
    {
        return Ok(await _service.UpdateAutoAssignOrdersAsync(userid, false));
    }

    /// <summary>
    /// update Auto assign Orders.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("enableavailableorders/{userid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> AutoAvailableOrdersAsync(Guid userid)
    {
        return Ok(await _service.UpdateAvailableForOrderAsync(userid, true));
    }

    /// <summary>
    /// update Auto assign Orders.
    /// </summary>
    /// <response code="200">User Setting updated.</response>
    /// <response code="404">User Setting not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("disableavailableorders/{userid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Setting", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Settings.Update)]
    public async Task<IActionResult> DisableAvailablenOrdersAsync(Guid userid)
    {
        return Ok(await _service.UpdateAvailableForOrderAsync(userid, false));
    }
}
