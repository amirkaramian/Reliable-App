using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.MassEmails.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.MassEmail;

namespace MyReliableSite.Admin.API.Controllers.MassEmail;

public class MassEmailsController : BaseController
{
    private readonly IMassEmailService _massEmailService;

    public MassEmailsController(IMassEmailService massEmailService)
    {
        _massEmailService = massEmailService ?? throw new ArgumentNullException(nameof(massEmailService));
    }

    /// <summary>
    /// Create an SmtpConfiguration.
    /// </summary>
    /// <response code="200">SmtpConfiguration created.</response>
    /// <response code="400">SmtpConfiguration already exists.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [ProducesResponseType(typeof(Result<bool>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Create)]
    [SwaggerHeader("tenant", "SmtpConfigurations", "Create", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> SendEmailAsync(MassEmailSendRequest request)
    {
        return Ok(await _massEmailService.SendEmailAsync(request));
    }
}
