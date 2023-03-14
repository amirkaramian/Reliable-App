using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Brands.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;

namespace MyReliableSite.Client.API.Controllers.Identity;
[ApiController]
[Route("api/[controller]")]
public sealed class IdentityController : ControllerBase
{
    private readonly ICurrentUser _user;
    private readonly IIdentityService _identityService;
    private readonly IUserService _userService;
    private readonly IBrandService _brandService;
    private readonly IConfiguration _config;

    public IdentityController(IIdentityService identityService, ICurrentUser user, IUserService userService, IBrandService brandService, IConfiguration config)
    {
        _identityService = identityService;
        _user = user;
        _userService = userService;
        _brandService = brandService;
        _config = config;
    }

    [HttpPost("verifyRecaptcha")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    public async Task<IActionResult> verifyRecaptchaAsync(ReCaptchaClientRequest request)
    {
        string origin = GenerateOrigin();
        return Ok(await _identityService.verifyRecaptchaAsync(request));
    }

    [HttpPost("register-client-user")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
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
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> UpdateEmailAsync(UpdateEmailRequest request)
    {
        return Ok(await _identityService.UpdateEmailAsync(request));
    }

    [HttpGet("confirm-phone-number")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmPhoneNumberAsync([FromQuery] string userId, [FromQuery] string code)
    {
        return Ok(await _identityService.ConfirmPhoneNumberAsync(userId, code));
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        string origin = GenerateOrigin();
        return Ok(await _identityService.ForgotPasswordAsync(request, origin));
    }

    [HttpPost("reset-password")]
    [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
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

    [HttpPut("profile")]
    [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    public async Task<IActionResult> UpdateProfileAsync(UpdateProfileRequest request)
    {
        return Ok(await _identityService.UpdateProfileAsync(request));
    }

    [HttpGet("profile")]
    [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> GetProfileDetailsAsync()
    {
        return Ok(await _userService.GetUserProfileAsync());
    }

    /// <summary>
    /// retrive the Brand against specific id for logout user.
    /// </summary>
    /// <response code="200">Brand returns.</response>
    /// <response code="400">Brand not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("brand/{brandId:guid}")]
    [SwaggerHeader("tenant", "Brands", "Read", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [AllowAnonymous]
    public async Task<IActionResult> GetBrandLogout(Guid brandId) => Ok(await _brandService.GetBrandLogout(brandId));

    /* [HttpDelete]
     [SwaggerHeader("tenant", "Identity", "", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
     public async Task<IActionResult> DeleteAsync()
     {
         return Ok(await _userService.DeleteUserAccountAsync());
     }*/

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
