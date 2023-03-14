using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Identity.Interfaces;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.MFA;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Client.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public class MFAuthenticatorController : ControllerBase
{
    private readonly IMFAuthenticatorService _iMFAuthenticatorService;

    public MFAuthenticatorController(IMFAuthenticatorService iMFAuthenticatorService)
    {
        _iMFAuthenticatorService = iMFAuthenticatorService;
    }

    [HttpPost("Validate-MFA")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> ValidateMFACodeAndAddApp(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var token = await _iMFAuthenticatorService.ValidateMFACodeAndAddApp(enableAuthenticatorRequest, false, GenerateIPAddress());
        return Ok(token);
    }

    [HttpPost("Validate-OTP-2FA")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Identity", "View", "Input your tenant to access this API i.e. admin for test", "admin", true, false, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> ValidateOTP2FA(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var token = await _iMFAuthenticatorService.Validate2FAEnable2FAByEmail(enableAuthenticatorRequest);
        return Ok(token);
    }

    [HttpPost("Get-MFA-Key")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> GETqRCodeUriForAuthenticatorApp(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.GETqRCodeUriForAuthenticatorApp(enableAuthenticatorRequest));
    }

    [HttpPost("GenerateOTPEmail")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to Generate OTP Email.")]
    public async Task<IActionResult> GenerateOTPEmail(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var message = await _iMFAuthenticatorService.GenerateOTPEmail(enableAuthenticatorRequest);
        return Ok(message);
    }

    [HttpPost("Enable-Disable-2fa")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to Generate OTP Email.")]
    public async Task<IActionResult> EnableDisable2fa(EnableDisableAuthenticatorRequest enableAuthenticatorRequest)
    {
        var message = await _iMFAuthenticatorService.EnableDisable2fa(enableAuthenticatorRequest);
        return Ok(message);
    }

    [HttpPost("GetCurrentStatusOfTwoFactorAuthentication")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> GetCurrentStatusOfTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.GetCurrentStatusOfTwoFactorAuthentication(enableAuthenticatorRequest));
    }

    [HttpPost("RemoveTwoFactorAuthentication")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> RemoveTwoFactorAuthentication(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.RemoveTwoFactorAuthentication(enableAuthenticatorRequest));
    }

    [HttpPost("ResetAuthenticator")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> ResetAuthenticator(EnableAuthenticatorRequest enableAuthenticatorRequest)
    {
        return Ok(await _iMFAuthenticatorService.ResetAuthenticator(enableAuthenticatorRequest));
    }

    [HttpPost("ResetGenerateRecoveryCodes")]
    [AllowAnonymous]
    [SwaggerHeader("tenant", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
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
