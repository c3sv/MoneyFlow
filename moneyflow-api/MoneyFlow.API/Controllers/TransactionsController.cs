using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Transactions;
using MoneyFlow.Application.Transactions.CreateTransaction;
using MoneyFlow.Application.Transactions.GetTransactions;
using MoneyFlow.Application.Transactions.DeleteTransaction;
using MoneyFlow.Application.Transactions.GetTransactionById;
using MoneyFlow.Application.Transactions.UpdateTransaction;


namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/transactions")]
public sealed class TransactionsController : ControllerBase
{
    private readonly CreateTransactionHandler _createTransactionHandler;
    private readonly GetTransactionsHandler _getTransactionsHandler;
    private readonly GetTransactionByIdHandler _getTransactionByIdHandler;
    private readonly UpdateTransactionHandler _updateTransactionHandler;
    private readonly DeleteTransactionHandler _deleteTransactionHandler;
    
    public TransactionsController(
        CreateTransactionHandler createTransactionHandler,
        GetTransactionsHandler getTransactionsHandler,
        GetTransactionByIdHandler getTransactionByIdHandler,
        UpdateTransactionHandler updateTransactionHandler,
        DeleteTransactionHandler deleteTransactionHandler)
    {
        _createTransactionHandler = createTransactionHandler;
        _getTransactionsHandler = getTransactionsHandler;
        _getTransactionByIdHandler = getTransactionByIdHandler;
        _updateTransactionHandler = updateTransactionHandler;
        _deleteTransactionHandler = deleteTransactionHandler;
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
                transaction.AccountId,
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
            request.AccountId,
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
    [HttpGet("{transactionId:long}")]
    [ProducesResponseType(
        typeof(TransactionResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponse>> GetById(
        long transactionId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetTransactionByIdQuery(
            userId,
            transactionId);

        var result = await _getTransactionByIdHandler.HandleAsync(
            query,
            cancellationToken);

        var response = new TransactionResponse(
            result.Id,
            result.AccountId,
            result.CategoryId,
            result.Amount,
            result.Description,
            result.Date,
            result.Type);

        return Ok(response);
    }
    
    [HttpPut("{transactionId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        long transactionId,
        [FromBody] UpdateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new UpdateTransactionCommand(
            userId,
            transactionId,
            request.AccountId,
            request.CategoryId,
            request.Amount,
            request.Description,
            request.Date,
            request.Type);

        await _updateTransactionHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
    
    [HttpDelete("{transactionId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long transactionId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new DeleteTransactionCommand(
            userId,
            transactionId);

        await _deleteTransactionHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
}