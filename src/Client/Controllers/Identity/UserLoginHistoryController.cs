using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Client.API.Controllers.Identity;
[Route("api/[controller]")]
[ApiController]
public class UserLoginHistoryController : BaseController
{
    private readonly IUserLoginHistoryService _service;
    private readonly IConfiguration _config;
    public UserLoginHistoryController(IUserLoginHistoryService service, IConfiguration config)
    {
        _service = service;
        _config = config;
    }

    /// <summary>
    /// retrive the User Histories against specific id.
    /// </summary>
    /// <response code="200">User Histories returns.</response>
    /// <response code="400">User History not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserLoginHistoryDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("loginhistorybyuserid/{userid}")]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false)]
    [MustHavePermission(PermissionConstants.UserLoginHistory.View)]
    public async Task<IActionResult> GetLoginHistoryByUserIdAsync(string userid)
    {
        var userLoginHistories = await _service.GetUserLoginHistoryByUserIdAsync(userid);
        return Ok(userLoginHistories);
    }
}
