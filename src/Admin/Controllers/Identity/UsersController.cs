using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;
using MyReliableSite.Shared.DTOs.Notifications;

namespace MyReliableSite.Admin.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService, ICurrentUser user)
    {
        _userService = userService;
    }

    /// <summary>
    /// List of records of all users.
    /// </summary>
    /// <response code="200">User Details List returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserDetailsDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAllAsync()
    {
        var users = await _userService.GetAllAsync();
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
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public IActionResult GetAllonlineUser()
    {
        return Ok(CurrentOnlineUser.OnlineUsers.ToList());
    }

    /// <summary>
    /// List of records of all users against a specific admin group id.
    /// </summary>
    /// <response code="200">User Details List returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserDetailsDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("GetAllUsersofAdminGroup/{admingroupid}")]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAllUsersofAdminGroupAsync(string admingroupid)
    {
        var users = await _userService.GetAllUsersofAdminGroupAsync(admingroupid);
        return Ok(users);
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
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAllAsync(string rolename)
    {
        var users = await _userService.GetAllByUserRoleAsync(rolename);
        return Ok(users);
    }

    /// <summary>
    /// retrive the User Details against specific id.
    /// </summary>
    /// <response code="200">User Details returns.</response>
    /// <response code="400">User Details not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<UserDetailsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var user = await _userService.GetAsync(id);
        return Ok(user);
    }

    /// <summary>
    /// retrive the User Role against specific id.
    /// </summary>
    /// <response code="200">User Role returns.</response>
    /// <response code="400">User Role not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<UserRolesResponse>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}/roles")]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetRolesAsync(string id)
    {
        var userRoles = await _userService.GetRolesAsync(id);
        return Ok(userRoles);
    }

    /// <summary>
    /// retrive the permissions against specific id.
    /// </summary>
    /// <response code="200">permissions returns.</response>
    /// <response code="400">permissions not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<PermissionDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("{id}/permissions")]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetPermissionsAsync(string id)
    {
        var userPermissions = await _userService.GetPermissionsAsync(id);
        return Ok(userPermissions);
    }

    /// <summary>
    /// Assign roles to a user.
    /// </summary>
    /// <response code="200">roles assignment successful or failed with error message.</response>
    /// <response code="400">Failed.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPost]
    [ProducesResponseType(typeof(IResult<string>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("{id}/roles")]
    public async Task<IActionResult> AssignRolesAsync(string id, UserRolesRequest request)
    {
        var result = await _userService.AssignRolesAsync(id, request);
        return Ok(result);
    }

    /// <summary>
    /// Delete a specific User by unique id.
    /// </summary>
    /// <response code="200">User deleted.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpDelete("{id}/Delete")]
    [SwaggerHeader("tenant", "Identity", "Remove", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Remove)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        return Ok(await _userService.DeleteUserAsync(id));
    }

    /// <summary>
    /// deactivate the user.
    /// </summary>
    /// <response code="200">user deactivated.</response>
    /// <response code="404">user not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id}/Deactivate")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> DeactiveAsync(string id)
    {
        return Ok(await _userService.DeactiveUserAsync(id));
    }

    /// <summary>
    /// activate the user.
    /// </summary>
    /// <response code="200">User Activated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("{id}/Activate")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> ActiveAsync(string id)
    {
        return Ok(await _userService.ActiveUserAsync(id));
    }

    /// <summary>
    /// update the user to take orders.
    /// </summary>
    /// <response code="200">User updated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("activateadminusertotakeorder/{adminuserid}")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> ActivateUserTakeOrder(string adminuserid)
    {
        return Ok(await _userService.ActivateUserTakeOrder(adminuserid));
    }

    /// <summary>
    /// update the user to not take orders.
    /// </summary>
    /// <response code="200">User updated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("deactivateadminusertotakeorder/{adminuserid}")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> DeActivateUserTakeOrder(string adminuserid)
    {
        return Ok(await _userService.DeActivateUserTakeOrder(adminuserid));
    }

    /// <summary>
    /// update the user for available orders.
    /// </summary>
    /// <response code="200">User updated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("activateadminuserforavailableorder/{adminuserid}")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> ActivateUserForAvailableOrder(string adminuserid)
    {
        return Ok(await _userService.ActivateUserAvailableForOrders(adminuserid));
    }

    /// <summary>
    /// update the user to not available for orders.
    /// </summary>
    /// <response code="200">User updated.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<string>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("deactivateadminuserforavailableorder/{adminuserid}")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> DeActivateUseForAvailableOrder(string adminuserid)
    {
        return Ok(await _userService.DeActivateUserAvailableForOrders(adminuserid));
    }

    /// <summary>
    /// List of records of all users who can take orders.
    /// </summary>
    /// <response code="200">User Details List returns.</response>
    /// <response code="400">Article not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<List<UserDetailsDto>>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet("getalluserstotakeorders")]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAllUsersToTakeOrdersAsync()
    {
        var users = await _userService.GetAllUsersToTakeOrdersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Find the User based on some conditions like Get List of User having Bills, Tickets, Orders etc greater than 100 etc.
    /// </summary>
    /// <response code="200">User returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("find/specific")]
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Users.ListAll)]
    [SwaggerHeader("tenant", "Identity", "ListAll", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetUsersBasedOnConditionsAsync(UsersBasedOnConditionsRequest request)
    {
        return Ok(await _userService.GetUsersBasedOnConditionsForNotificationTemplates(request));
    }
}
