using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Identity;

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
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">User login histories returns.</response>
    /// <response code="400">History not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(PaginatedResult<UserLoginHistoryDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("search")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.UserLoginHistory.Search)]
    [SwaggerOperation(Summary = "Search UserLoginHistory using available Filters.")]
    public async Task<IActionResult> SearchAsync(UserLoginHistoryListFilter filter)
    {
        var userLoginHistory = await _service.SearchAsync(filter);
        return Ok(userLoginHistory);
    }

    /// <summary>
    /// retrive the User History against specific id.
    /// </summary>
    /// <response code="200">User History.</response>
    /// <response code="400">User History not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<UserLoginHistoryDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.UserLoginHistory.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var userLoginHistory = await _service.GetUserLoginHistoryAsync(id);
        return Ok(userLoginHistory);
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
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.UserLoginHistory.View)]
    public async Task<IActionResult> GetLoginHistoryByUserIdAsync(string userid)
    {
        var userLoginHistories = await _service.GetUserLoginHistoryByUserIdAsync(userid);
        return Ok(userLoginHistories);
    }

    /// <summary>
    /// Create an User History.
    /// </summary>
    /// <response code="200">User History created.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(500)]
    [HttpPost]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.UserLoginHistory.Create)]
    public async Task<IActionResult> CreateAsync(CreateUserLoginHistoryRequest request)
    {
        return Ok(await _service.CreateUserLoginHistoryAsync(request));
    }

}
