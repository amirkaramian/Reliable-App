using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.WHMCS;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.WHMCS;
using IResult = MyReliableSite.Application.Wrapper.IResult;

namespace MyReliableSite.Admin.API.Controllers.WHMCS;
[Route("api/[controller]")]
[ApiController]
public class WHMCSimportController : BaseController
{

    private readonly IWHMCSImportService _iWHMCSService;
    public WHMCSimportController(IWHMCSImportService iWHMCSService)
    {
        _iWHMCSService = iWHMCSService;
    }

    [ProducesResponseType(typeof(Result<ImportWHMSCResponse>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("ValidateTheData")]
    [SwaggerHeader("tenant", "WHMCS", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.WHMCS.View)]
    public async Task<IActionResult> ValidateTheDataAsync(ImportWHMSCRequest request)
    {
        return Ok(await _iWHMCSService.ValidateTheData(request));
    }

    [ProducesResponseType(typeof(List<IResult>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpPost("ImportData")]
    [SwaggerHeader("tenant", "WHMCS", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.WHMCS.View)]
    public async Task<IActionResult> ImportDataAsync(ImportWHMSCRequest request)
    {
        string origin = GenerateOrigin();
        return Ok(await _iWHMCSService.ImportData(request, origin));
    }

    private string GenerateOrigin()
    {
        string baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}{this.Request.PathBase.Value}";
        string origin = string.IsNullOrEmpty(Request.Headers["origin"].ToString()) ? baseUrl : Request.Headers["origin"].ToString();
        return origin;
    }
}
