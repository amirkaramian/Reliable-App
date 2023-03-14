using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Dashboard.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs;
using MyReliableSite.Shared.DTOs.Dashboard;

namespace MyReliableSite.Client.API.Controllers.Dashboard;

public class DashboardController : BaseController
{
    private readonly IDashboardService _service;
    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    /// <summary>
    /// retrive the Stats.
    /// </summary>
    /// <response code="200">Stats returns.</response>
    /// <response code="400">Stats not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<ClientDashboardDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet]
    [SwaggerHeader("tenant", "Dashboard", "View", "Input your tenant to access this API i.e. client for test", "client", true)]
    public async Task<IActionResult> GetClientDataAsync()
    {
        var detail = await _service.GetCleintDataAsync();
        return Ok(detail);
    }
}
