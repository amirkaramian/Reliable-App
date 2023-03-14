using MaintenanceModeMiddleware.Configuration.Enums;
using MaintenanceModeMiddleware.Configuration.State;
using MaintenanceModeMiddleware.Services;
using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Maintenances.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Domain.MaintenanceMode;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Maintenance;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Maintenance;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : BaseController
{
    private readonly IMaintenanceControlService _maintenanceCtrlSvc;
    private readonly IMaintenanceService _maintenanceService;

    public MaintenanceController(IMaintenanceControlService maintenanceCtrlSvc, IMaintenanceService maintenanceService)
    {
        _maintenanceCtrlSvc = maintenanceCtrlSvc;
        _maintenanceService = maintenanceService;
    }

    /// <summary>
    /// Toggle Maintenance Mode.
    /// </summary>
    /// <response code="200">ToggleMaintenanceMode successful.</response>
    [HttpPost]
    [ProducesResponseType(typeof(Result), 200)]
    [HttpPost("ToggleMaintenanceMode")]
    [SwaggerHeader("tenant", "Maintenance", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Admin.Maintenance)]
    public IActionResult ToggleMaintenanceMode(MaintenanceRequest maintenanceRequest)
    {
        var maintenanceState = _maintenanceCtrlSvc.GetState();

        if (maintenanceState.IsMaintenanceOn)
        {
            _maintenanceCtrlSvc.LeaveMaintanence();
        }
        else
        {
            _maintenanceCtrlSvc.EnterMaintanence(maintenanceRequest.ExpirationDateTime, options =>
            {
                options.UseResponse(maintenanceRequest.Message, ResponseContentType.Text, System.Text.Encoding.UTF8);
                if (maintenanceRequest.ByPassuserRoles != null && maintenanceRequest.ByPassuserRoles.Count() > 0)
                {
                    options.BypassUserRoles(maintenanceRequest.ByPassuserRoles);

                }

                foreach (string user in maintenanceRequest.ByPassUsers)
                {
                    if (!string.IsNullOrEmpty(user))
                        options.BypassUser(user);

                }
            });
        }

        _maintenanceService.CreateMaintenanceAsync(new CreateMaintenanceRequest()
        {
            Message = maintenanceRequest.Message,
            Status = maintenanceRequest.Status,
            ExpirationDateTime = maintenanceRequest.ExpirationDateTime,
            ByPassUsers = string.Join(",", maintenanceRequest.ByPassUsers),
            ByPassuserRoles = string.Join(",", maintenanceRequest?.ByPassuserRoles)
        });

        return Ok(new Result { Succeeded = true });
    }

    /// <summary>
    /// retrive the current status of MaintenanceMode Log.
    /// </summary>
    /// <response code="200">Article returns.</response>
    [ProducesResponseType(typeof(IMaintenanceState), 200)]
    [HttpGet("MaintenanceMode/{tenant}")]
    [SwaggerHeader("tenant", "Maintenance", "Get", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Admin.Maintenance)]
    [SwaggerOperation(Summary = "Get Current Status of Maintenance Mode")]
    public async Task<IActionResult> MaintenanceMode(string tenant)
    {
        var maintenanceState = _maintenanceCtrlSvc.GetState();
        var lastMLog = await _maintenanceService.GetLastMaintenanceLogAsync(tenant);
        return Ok(new
        {
            Reason = lastMLog.Data != null ? lastMLog.Data.Message : string.Empty,
            IsMaintenanceOn = maintenanceState.IsMaintenanceOn,
            IsExpirationDateSpecified = maintenanceState.ExpirationDate != null,
            ExpirationDate = maintenanceState.ExpirationDate != null
                ? maintenanceState.ExpirationDate
                : lastMLog.Data != null ? lastMLog.Data.ExpirationDateTime : null
        });
    }

    /// <summary>
    /// retrive the MaintenanceModeAllLogs.
    /// </summary>
    /// <response code="200">Article returns.</response>
    [ProducesResponseType(typeof(List<MaintenanceMode>), 200)]
    [HttpGet("MaintenanceModeAllLogs/{tenant}")]
    [SwaggerHeader("tenant", "Maintenance", "Get", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Admin.Maintenance)]
    [SwaggerOperation(Summary = "Get Current Status of Maintenance Mode")]
    public async Task<IActionResult> MaintenanceModeAllLogs(string tenant)
    {
        return Ok(await _maintenanceService.GetMaintenanceLogsAsync(tenant));
    }
}