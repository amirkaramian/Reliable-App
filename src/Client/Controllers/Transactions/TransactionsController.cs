using Microsoft.AspNetCore.Mvc;
using MyReliableSite.Application.Transactions.Interfaces;
using MyReliableSite.Application.Wrapper;
using MyReliableSite.Client.API.Controllers;
using MyReliableSite.Domain.Constants;
using MyReliableSite.Infrastructure.Identity.Permissions;
using MyReliableSite.Infrastructure.Swagger;
using MyReliableSite.Shared.DTOs.Transaction;
using Swashbuckle.AspNetCore.Annotations;

namespace MyReliableSite.Admin.API.Controllers.Transactions;
public class TransactionsController : BaseController
{
    private readonly ITransaction _transactionService;

    public TransactionsController(ITransaction transactionService)
    {
        _transactionService = transactionService;
    }

    /// <summary>
    /// Retrive the transaction against specific id.
    /// </summary>
    /// <response code="200">Transaction returns.</response>
    /// <response code="404">Transaction not found.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Result<TransactionDetailsDto>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Transactions", "View", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Transactions.View)]
    public async Task<IActionResult> GetAsync(Guid id)
    {
        var transaction = await _transactionService.GetTransactionDetailsAsync(id);
        return Ok(transaction);
    }

    /// <summary>
    /// List of records with pagination &amp; filters.
    /// </summary>
    /// <response code="200">Transaction List returns.</response>
    /// <response code="500">Oops! Can't lookup your record right now.</response>
    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResult<TransactionDto>), 200)]
    [ProducesResponseType(500)]
    [SwaggerHeader("tenant", "Transactions", "Search", "Input your tenant to access this API i.e. client", "client", true)]
    [MustHavePermission(PermissionConstants.Transactions.Search)]
    [SwaggerOperation(Summary = "Search Transaction using available Filters.")]
    public async Task<IActionResult> SearchAsync(TransactionListFilter filter)
    {
        var transaction = await _transactionService.SearchAsync(filter);
        return Ok(transaction);
    }

    /// <summary>
    /// Update a specific Transaction by unique id.
    /// </summary>
    /// <response code="200">Transaction updated.</response>
    /// <response code="404">Billing Settings not found.</response>
    /// <response code="404">Transaction not found.</response>
    /// <response code="500">Oops! Can't lookup your product right now.</response>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(Result<Guid>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    [MustHavePermission(PermissionConstants.Transactions.Update)]
    [SwaggerHeader("tenant", "Transactions", "Update", "Input your tenant to access this API i.e. admin for test", "admin", true)]
    public async Task<IActionResult> UpdateAsync(UpdateTransactionRequest request, Guid id)
    {
        return Ok(await _transactionService.UpdateTransactionAsync(request, id));
    }
}
