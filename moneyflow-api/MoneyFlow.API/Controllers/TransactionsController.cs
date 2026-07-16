using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Transactions;
using MoneyFlow.Application.Transactions.CreateTransaction;
using MoneyFlow.Application.Transactions.GetTransactions;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/transactions")]
public sealed class TransactionsController : ControllerBase
{
    private readonly CreateTransactionHandler _createTransactionHandler;
    private readonly GetTransactionsHandler _getTransactionsHandler;

    public TransactionsController(
        CreateTransactionHandler createTransactionHandler,
        GetTransactionsHandler getTransactionsHandler)
    {
        _createTransactionHandler = createTransactionHandler;
        _getTransactionsHandler = getTransactionsHandler;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<TransactionResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<TransactionResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetTransactionsQuery(userId);

        var results = await _getTransactionsHandler.HandleAsync(
            query,
            cancellationToken);

        var response = results
            .Select(transaction => new TransactionResponse(
                transaction.Id,
                transaction.CategoryId,
                transaction.Amount,
                transaction.Description,
                transaction.Date,
                transaction.Type))
            .ToList();

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateTransactionResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateTransactionResponse>> Create(
        [FromBody] CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new CreateTransactionCommand(
            userId,
            request.CategoryId,
            request.Amount,
            request.Description,
            request.Date,
            request.Type);

        var transactionId =
            await _createTransactionHandler.HandleAsync(
                command,
                cancellationToken);

        var response =
            new CreateTransactionResponse(transactionId);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }
}