using Microsoft.AspNetCore.Mvc;

using MyReliableSite.Application.SmtpConfigurations.Interfaces;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;

namespace MyReliableSite.Client.API.Controllers.SmtpConfiguration;

public class SmtpConfigurationsController : BaseController
{
    private readonly ISmtpConfigurationService _smtpConfigurationService;

    public SmtpConfigurationsController(ISmtpConfigurationService smtpConfigurationService)
    {
        _smtpConfigurationService = smtpConfigurationService;
    }

    [HttpGet("{id:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Read)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> GetAsync(Guid id) => Ok(await _smtpConfigurationService.GetAsync(id));

    [HttpGet("{brandId:guid}")]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.Read)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> GetByBrandAsync(Guid brandId) => Ok(await _smtpConfigurationService.GetByBrandAsync(brandId));

    [HttpGet]
    [MustHavePermission(PermissionConstants.SmtpConfigurations.ReadAll)]
    [SwaggerHeader("tenant,gen-api-key", "Articles", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true, true)]
    public async Task<IActionResult> SearchAsync() => Ok(await _smtpConfigurationService.GetAllAsync());
}