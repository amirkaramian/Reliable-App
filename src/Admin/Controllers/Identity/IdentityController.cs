using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Admin.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public sealed class IdentityController : ControllerBase
{
    private readonly ICurrentUser _user;
    private readonly IIdentityService _identityService;
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public IdentityController(IIdentityService identityService, ICurrentUser user, IUserService userService, IConfiguration config)
    {
        _identityService = identityService;
        _user = user;
        _userService = userService;
        _config = config;
    }

    /// <summary>
    /// Verify the recaptcha an Article.
    /// </summary>
    /// <response code="200">verified.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>\
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(500)]
    [HttpPost("verifyRecaptcha")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    public async Task<IActionResult> verifyRecaptchaAsync(ReCaptchaClientRequest request)
    {
        return Ok(await _identityService.verifyRecaptchaAsync(request));
    }

    /*[HttpPost("register")]
    [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Identity.Register)]
    public async Task<IActionResult> RegisterAsync(RegisterRequest request)
    {
        string origin = GenerateOrigin();
        return Ok(await _identityService.RegisterAsync(request, origin));
    }*/

    /// <summary>
    /// Create an admin user.
    /// </summary>
    /// <response code="200">Admin user created.</response>
    /// <response code="500">user/Email already exists.</response>\
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(500)]
    [HttpPost("register-admin")]
    [SwaggerHeader("tenant", "Identity", "Register", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Identity.Register)]
    public async Task<IActionResult> RegisterAdminAsync(RegisterAdminRequest request)
    {
        string origin = _config.GetValue<string>("MiddlewareSettings:AdminOrigin");
        return Ok(await _identityService.RegisterAdminAsync(request, origin));
    }

    /// <summary>
    /// Create a client user.
    /// </summary>
    /// <response code="200">Client user created.</response>
    /// <response code="500">user/Email already exists.</response>\
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(500)]
    [HttpPost("register-client-user")]
    [SwaggerHeader("tenant", "Identity", "Register", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Identity.Register)]
    public async Task<IActionResult> RegisterClientUserAsync(RegisterClientRequest request)
    {
        string origin = _config.GetValue<string>("MiddlewareSettings:ClientOrigin");
        return Ok(await _identityService.RegisterClientUserAsync(request, origin));
    }

    /// <summary>
    /// confirm the email.
    /// </summary>
    /// <response code="200">Email confirmation success.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Application.Wrapper.IResult<string>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Identity", "Register", "Input your tenant to access this API i.e. admin for test", "", true)]
    [HttpGet("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code, [FromQuery] string tenant)
    {
        return Ok(await _identityService.ConfirmEmailAsync(userId, code, tenant));
    }

    /// <summary>
    /// confirm phone number.
    /// </summary>
    /// <response code="200">Phone number confirmation.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Application.Wrapper.IResult<string>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Identity", "Register", "Input your tenant to access this API i.e. admin for test", "", true)]
    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return Ok(await _identityService.ConfirmPhoneNumberAsync(userId, code));
    }

    /// <summary>
    /// forgot password request.
    /// </summary>
    /// <response code="200">Forgot password submitted successfull.</response>
    /// <response code="404">User doesn't exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>\
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        string origin = GenerateOrigin();
        return Ok(await _identityService.ForgotPasswordAsync(request, origin));
    }

    /// <summary>
    /// reset the password.
    /// </summary>
    /// <response code="200">password reset requests successfull.</response>
    /// <response code="404">User doesn't exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>\
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("reset-password")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        return Ok(await _identityService.ResetPasswordAsync(request));
    }

    /// <summary>
    /// Update user password.
    /// </summary>
    /// <response code="200">User change password successfull.</response>
    /// <response code="404">User doesn't exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>\
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("change-password")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePasswordAsync(ChangePasswordRequest request)
    {
        return Ok(await _identityService.ChangePasswordAsync(request));
    }

    /// <summary>
    /// Update user password.
    /// </summary>
    /// <response code="200">User change password successfull.</response>
    /// <response code="404">User doesn't exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>\
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("change-password-other")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> ChangePasswordOtheruserAsync(ChangePasswordOtherUserRequest request)
    {
        return Ok(await _identityService.ChangePasswordOtherUserAsync(request));
    }

    /// <summary>
    /// update a specific user profile by unique id.
    /// </summary>
    /// <response code="200">user profile updated.</response>
    /// <response code="404">user profile not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("profile")]
    [MustHavePermission(PermissionConstants.Users.Update)]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateProfileAsync(UpdateProfileRequest request)
    {
        return Ok(await _identityService.UpdateProfileAsync(request));
    }

    /// <summary>
    /// update a specific user profile by unique id.
    /// </summary>
    /// <response code="200">user profile updated.</response>
    /// <response code="404">user profile not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("updateemail")]
    [MustHavePermission(PermissionConstants.Users.Update)]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateEmailAsync(UpdateEmailRequest request)
    {
        return Ok(await _identityService.UpdateEmailAsync(request));
    }

    /// <summary>
    /// update a specific user profile by unique id.
    /// </summary>
    /// <response code="200">user profile updated.</response>
    /// <response code="404">user profile not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Application.Wrapper.IResult), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [HttpPut("profile/{id}")]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.Update)]
    public async Task<IActionResult> UpdateProfileAsync(UpdateProfileRequest request, string id)
    {
        return Ok(await _identityService.UpdateProfileAsync(request, id));
    }

    /// <summary>
    /// retrive the user profile against specific id.
    /// </summary>
    /// <response code="200">user profile returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Application.Wrapper.IResult<UserDetailsDto>), 200)]
    [ProducesResponseType(500)]
    [HttpGet("profile/{id}")]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Users.View)]

    public async Task<IActionResult> GetProfileDetailsAsync(string id)
    {
        return Ok(await _userService.GetUserProfileAsync(id));
    }

    /// <summary>
    /// retrive the user profile against specific id.
    /// </summary>
    /// <response code="200">user profile returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(IResult<UserDetailsDto>), 200)]
    [ProducesResponseType(500)]
    [HttpGet("profile")]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetProfileDetailsAsync()
    {
        return Ok(await _userService.GetUserProfileAsync());
    }

    private string GenerateOrigin()
    {
        string baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{this.Request.PathBase.Value}";
        string origin = string.IsNullOrEmpty(Request.Headers["origin"].ToString()) ? baseUrl : Request.Headers["origin"].ToString();
        return origin;
    }

    private string GenerateIPAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"];
        }
        else
        {
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }
    }
}
