using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Abstractions.Services.Identity;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Identity.Services;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Identity;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace MyReliableSite.Client.API.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
public sealed class TokensController : ControllerBase
{
    // this commit is to deploy api
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;

    public TokensController(ITokenService tokenService, IUserService userService)
    {
        _tokenService = tokenService;
        _userService = userService;
    }

    [HttpPost]
    [AllowAnonymous]
    [SwaggerHeader("tenant,gen-api-key,devicename,X-Forwarded-For,location", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> GetTokenAsync(TokenRequest request)
    {
        var token = await _tokenService.GetTokenAsync(request, GenerateIPAddress(), GetDeviceName(), GetLocation(), false, GenerateOrigin());
        return Ok(token);
    }

    [HttpPost("GetTokenByOTP")]
    [AllowAnonymous]
    [SwaggerHeader("tenant,gen-api-key,devicename,X-Forwarded-For,location", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key to generate valid Access Token.")]
    public async Task<IActionResult> GetTokenByOTPAsync(OTPRequest request)
    {
        var token = await _tokenService.GetTokenByOTPAsync(request, GenerateIPAddress(), GetDeviceName(), GetLocation(), false);
        return Ok(token);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true, false)]
    public async Task<ActionResult> RefreshAsync(RefreshTokenRequest request)
    {
        var response = await _tokenService.RefreshTokenAsync(request, GenerateIPAddress(), false);
        return Ok(response);
    }

    /// <summary>
    /// Login Admin As Client.
    /// </summary>
    /// <response code="200">Token information returned.</response>
    /// <response code="400">Tenant expired.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<TokenResponse>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 401)]
    [ProducesResponseType(500)]
    [HttpPost("LoginAdminAsClient")]
    [MustHavePermission(PermissionConstants.Admin.AdminLoginAsClient)]
    [SwaggerHeader("tenant,AdminAsClient", "Identity", "", "Input your tenant to access this API i.e. admin for test", "", true),]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key & Admin Id to generate valid Access Token to Login As Client.")]
    public async Task<IActionResult> LoginAdminAsClient(string clientId)
    {
        if (!Request.Headers.TryGetValue("AdminAsClient", out var adminAsClient))
        {
            var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
            response.Content = new StringContent("AdminAsClient Header is missing");
            return BadRequest(response);

        }
        else
        {
            var adminUser = await _userService.GetAsync(adminAsClient);
            if (adminUser == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
                response.Content = new StringContent("AdminAsClient Header has not valid value");
                return BadRequest(response);
            }
        }

        var token = await _tokenService.LoginAdminAsClientAsync(clientId, GenerateIPAddress());
        return Ok(token);
    }

    /// <summary>
    /// Login Client As Admin.
    /// </summary>
    /// <response code="200">Token information returned.</response>
    /// <response code="400">Tenant expired.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(IResult<TokenResponse>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 401)]
    [ProducesResponseType(500)]
    [HttpPost("LoginClientAsAdmin")]
    [MustHavePermission(PermissionConstants.Admin.AdminLoginAsClient)]
    [SwaggerHeader("tenant,ClientAsAdmin", "Identity", "", "Input your tenant to access this API i.e. admin for test", "", true),]
    [SwaggerOperation(Summary = "Submit Credentials with Tenant Key & client Id to generate valid Access Token to Login As Client.")]
    public async Task<IActionResult> LoginClientAsAdmin(string adminId)
    {
        if (!Request.Headers.TryGetValue("ClientAsAdmin", out var clientAsAdmin))
        {
            var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
            response.Content = new StringContent("ClientAsAdmin Header is missing");
            return BadRequest(response);

        }
        else
        {
            var adminUser = await _userService.GetAsync(adminId);
            if (adminUser == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.PreconditionFailed);
                response.Content = new StringContent("AdminAsClient Header has not valid value");
                return BadRequest(response);
            }
        }

        var token = await _tokenService.LoginClientAsAdminAsync(adminId, GenerateIPAddress());
        return Ok(token);
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

    private string GetDeviceName()
    {
        if (Request.Headers.ContainsKey("devicename"))
        {
            return Request.Headers["devicename"];
        }
        else
        {
            return "UnKnown";
        }
    }

    private string GetLocation()
    {
        if (Request.Headers.ContainsKey("location"))
        {
            return Request.Headers["location"];
        }
        else
        {
            return "UnKnown";
        }
    }

    private string GenerateOrigin()
    {
        string baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{this.Request.PathBase.Value}";
        string origin = string.IsNullOrEmpty(Request.Headers["origin"].ToString()) ? baseUrl : Request.Headers["origin"].ToString();
        return origin;
    }
}