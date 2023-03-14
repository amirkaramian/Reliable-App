using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.ManageModule.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.ManageModule;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.ManageModule;
[Route("api/[controller]")]
[ApiController]

public class UserModuleManagementController : BaseController
{
    private readonly IUserModuleManagementService _service;
    private readonly IConfiguration _config;
    public UserModuleManagementController(IUserModuleManagementService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// retrive the User Module Management against specific user id.
    /// </summary>
    /// <response code="200">User Module Management returns.</response>
    /// <response code="400">User Module Management not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserModuleDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getmodulebyuser/{userid}")]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.ModuleManagements.View)]
    public async Task<IActionResult> GetModuleByTenantAsync(string userid)
    {
        return Ok(await _service.GetUserModuleManagementByUserIdAsync(userid));
    }
}
