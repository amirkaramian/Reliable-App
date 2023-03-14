using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.MFA;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class MFAuthenticatorController : ControllerBase
{
    private readonly IMFAuthenticatorService _iMFAuthenticatorService;

    public MFAuthenticatorController(IMFAuthenticatorService iMFAuthenticatorService)
    {
        _iMFAuthenticatorService = iMFAuthenticatorService;
    }

    /// <summary>
    /// Get MFA information to valide &amp; add app.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(EnableAuthenticatorResponse), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 401)]
    [ProducesResponseType(500)]
    [HttpPost("Validate-MFA")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> ValidateMFACodeAndAddApp(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var token = await _iMFAuthenticatorService.ValidateMFACodeAndAddApp(enableAuthenticatorRequest, true, GenerateIPAddress());
        return Ok(token);
    }

    /// <summary>
    /// Validate OTP 2FA.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("Validate-OTP-2FA")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> ValidateOTP2FA(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var token = await _iMFAuthenticatorService.Validate2FAEnable2FAByEmail(enableAuthenticatorRequest);
        return Ok(token);
    }

    /// <summary>
    /// Get MFA information to valide &amp; add app.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(EnableAuthenticatorResponse), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("Get-MFA-Key")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> GETqRCodeUriForAuthenticatorApp(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.GETqRCodeUriForAuthenticatorApp(enableAuthenticatorRequest));
    }

    /// <summary>
    /// Generate OTP Email.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">User Not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("GenerateOTPEmail")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to Generate OTP Email.")]
    public async Task<IActionResult> GenerateOTPEmail(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var message = await _iMFAuthenticatorService.GenerateOTPEmail(enableAuthenticatorRequest);
        return Ok(message);
    }

    /// <summary>
    /// Enable/disable 2FA.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">User not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<string>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("Enable-Disable-2fa")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to Generate OTP Email.")]
    public async Task<IActionResult> EnableDisable2fa(EnableDisableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var message = await _iMFAuthenticatorService.EnableDisable2fa(enableAuthenticatorRequest);
        return Ok(message);
    }

    /// <summary>
    /// Get Current Status Of Two Factor Authentication.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">Not Found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(UserAuthenticatorStatus), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("GetCurrentStatusOfTwoFactorAuthentication")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> GetCurrentStatusOfTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.GetCurrentStatusOfTwoFactorAuthentication(enableAuthenticatorRequest));
    }

    /// <summary>
    /// Remove MFA.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">User Not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(bool), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("RemoveTwoFactorAuthentication")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> RemoveTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.RemoveTwoFactorAuthentication(enableAuthenticatorRequest));
    }

    /// <summary>
    /// Remove authenticator app.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">User Not Found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(UserAuthenticatorStatus), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("ResetAuthenticator")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> ResetAuthenticator(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.ResetAuthenticator(enableAuthenticatorRequest));
    }

    /// <summary>
    /// Get new recovery codes.
    /// </summary>
    /// <response code="200">Success.</response>
    /// <response code="404">User Not Found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(string[]), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 404)]
    [ProducesResponseType(500)]
    [HttpPost("ResetGenerateRecoveryCodes")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> ResetGenerateRecoveryCodes(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.ResetGenerateRecoveryCodes(enableAuthenticatorRequest));
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
