using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Transactions;
using MoneyFlow.Application.Transactions.CreateTransaction;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/transactions")]
public sealed class TransactionsController : ControllerBase
{
    private readonly CreateTransactionHandler
        _createTransactionHandler;

    public TransactionsController(
        CreateTransactionHandler createTransactionHandler)
    {
        _createTransactionHandler = createTransactionHandler;
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
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
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