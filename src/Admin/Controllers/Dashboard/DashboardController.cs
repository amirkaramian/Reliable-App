using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Dashboard;
using MyReliableSite.Application.Dashboard.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Dashboard;

namespace MyReliableSite.Admin.API.Controllers.Dashboard;

public class DashboardController : BaseController
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    /// <summary>
    /// retrive the Dashboard data against specific id.
    /// </summary>
    /// <response code="200">Dashboard data returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<DashboardDto>), 200)]
    [ProducesResponseType(500)]
    [HttpGet]
    [SwaggerHeader("tenant", "Dashboard", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetAsync()
    {
        var detail = await _service.GetAsync();
        return Ok(detail);
    }

    /// <summary>
    /// retrive the Dashboard data against specific id.
    /// </summary>
    /// <response code="200">Dashboard data returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<DataCountDto>), 200)]
    [ProducesResponseType(500)]
    [HttpGet("getDataCounts")]
    [SwaggerHeader("tenant", "Dashboard", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetDataCountsAsync()
    {
        var detail = await _service.GetDataCountsAsync();
        return Ok(detail);
    }
}
