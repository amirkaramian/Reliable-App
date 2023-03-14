using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Client.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var user = await _userService.GetAsync(id);
        return Ok(user);
    }

    [HttpGet("{id}/roles")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> GetRolesAsync(string id)
    {
        var userRoles = await _userService.GetRolesAsync(id);
        return Ok(userRoles);
    }

    [HttpGet("{id}/permissions")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> GetPermissionsAsync(string id)
    {
        var userPermissions = await _userService.GetPermissionsAsync(id);
        return Ok(userPermissions);
    }

    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRolesAsync(string id, UserRolesRequest request)
    {
        var result = await _userService.AssignRolesAsync(id, request);
        return Ok(result);
    }

    [HttpDelete("{id}/Delete")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(await _userService.DeleteUserAsync(id));
    }

    [HttpPut("{id}/Deactivate")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> DeactiveAsync(string id)
    {
        return Ok(await _userService.DeactiveUserAsync(id));
    }

    [HttpPut("{id}/Activate")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> ActiveAsync(string id)
    {
        return Ok(await _userService.ActiveUserAsync(id));
    }

    /// <summary>
    /// List of records of all users with specific to a role.
    /// </summary>
    /// <response code="200">User Details List returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserDetailsDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getallusersbyrolename/{rolename}")]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAllAsync(string rolename)
    {
        var users = await _userService.GetAllByUserRoleAsync(rolename);
        return Ok(users);
    }

    /// <summary>
    /// List of records of all users.
    /// </summary>
    /// <response code="200">User Details List returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<OnlineUser>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getallonlineuser")]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. client for test", "client", true)]
    public IActionResult GetAllonlineUser()
    {
        return Ok(CurrentOnlineUser.OnlineUsers.ToList());
    }
}
