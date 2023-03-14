using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Incomes.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Dashboard;
using MyReliableSite.Shared.DTOs.Incomes;
using MyReliableSite.Shared.DTOs.Transaction;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Incomes;
public class IncomesController : BaseController
{
    private readonly IIncomeService _incomeService;

    public IncomesController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    /// <summary>
    /// Retrive the incomes against specific id.
    /// </summary>
    /// <response code="200">Incomes returns.</response>
    /// <response code="404">Incomes not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<IncomeDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var income = await _incomeService.GetIncomingDetailsAsync(id);
        return Ok(income);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Incomes returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<IncomeDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "Search", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Transactions.Search)]
    [SwaggerOperation(Summary = "Search Income using available Filters.")]
    public async Task<IActionResult> SearchAsync(IncomeListFilter filter)
    {
        var incomes = await _incomeService.SearchAsync(filter);
        return Ok(incomes);
    }

    /// <summary>
    /// List of Income Overview records.
    /// </summary>
    /// <response code="200">Incomes Overview returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("IncomeOverview")]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    [SwaggerOperation(Summary = "Get Income Overview data.")]
    public async Task<IActionResult> IncomeOverview()
    {
        var incomes = await _incomeService.GetIncomeOverview();

        return Ok(incomes);
    }

    /// <summary>
    /// Income Forecast records.
    /// </summary>
    /// <response code="200">Incomes Forecast returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("IncomeForecast")]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    [SwaggerOperation(Summary = "Get Income Forecast data.")]
    public async Task<IActionResult> IncomeForecast()
    {
        var incomes = await _incomeService.GetIncomeForecast();

        return Ok(incomes);
    }

    /// <summary>
    /// Income Transaction Overview records.
    /// </summary>
    /// <response code="200">Incomes Transaction Overview returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("IncomeTransactionOverview/{transactionType}")]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    [SwaggerOperation(Summary = "Get Income Overview transactions data.")]
    public async Task<IActionResult> IncomeTransactionOverview(TransactionListFilter filter, string transactionType)
    {
        var incomes = await _incomeService.GetIncomeTransactionOverviewAsync(filter, transactionType);

        return Ok(incomes);
    }

    /// <summary>
    /// retrive history income.
    /// </summary>
    /// <response code="200">Dashboard data returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [ProducesResponseType(typeof(Result<DataCountDto>), 200)]
    [ProducesResponseType(500)]
    [HttpGet("annual")]
    [SwaggerHeader("tenant", "Incomes", "View", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> GetIncomeHistoryAsync()
    {
        var detail = await _incomeService.GetIncomingHistoryAsync();
        return Ok(detail);
    }
}
