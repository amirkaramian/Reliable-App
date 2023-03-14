using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Dashboard;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs;

namespace MyReliableSite.Admin.API.Controllers.Dashboard;

public class StatsController : BaseController
{
    private readonly IStatsService _service;

    public StatsController(IStatsService service)
    {
        _service = service;
    }

    /// <summary>
    /// retrive the Stats.
    /// </summary>
    /// <response code="200">Stats returns.</response>
    /// <response code="400">Stats not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<StatsDto>), 200)]
    [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
    [ProducesResponseType(500)]
    [HttpGet]
    [SwaggerHeader("tenant", "Dashboard", "View", "Input your tenant to access this API i.e. admin for test", "", true)]
    public async Task<IActionResult> GetAsync()
    {
        var stats = await _service.GetDataAsync();
        return Ok(stats);
    }
}