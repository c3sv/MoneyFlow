using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Accounts;
using MoneyFlow.Application.Accounts.CreateAccount;
using MoneyFlow.Application.Accounts.GetAccounts;
using MoneyFlow.Application.Accounts.UpdateAccountBalance;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly CreateAccountHandler _createAccountHandler;
    private readonly GetAccountsHandler _getAccountsHandler;
    private readonly UpdateAccountBalanceHandler _updateAccountBalanceHandler;

    public AccountsController(
        CreateAccountHandler createAccountHandler,
        GetAccountsHandler getAccountsHandler,
        UpdateAccountBalanceHandler updateAccountBalanceHandler)
    {
        _createAccountHandler = createAccountHandler;
        _getAccountsHandler = getAccountsHandler;
        _updateAccountBalanceHandler = updateAccountBalanceHandler;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateAccountResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateAccountResponse>> Create(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new CreateAccountCommand(
            userId,
            request.Bank,
            request.Nickname,
            request.Type,
            request.Last4,
            request.InitialBalance,
            request.Currency);

        var accountId = await _createAccountHandler.HandleAsync(
            command,
            cancellationToken);

        var response = new CreateAccountResponse(accountId);

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<AccountResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<AccountResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetAccountsQuery(userId);

        var results = await _getAccountsHandler.HandleAsync(
            query,
            cancellationToken);

        var response = results
            .Select(account => new AccountResponse(
                account.Id,
                account.Bank,
                account.Nickname,
                account.Type,
                account.Last4,
                account.Balance,
                account.Currency,
                account.UpdatedAt))
            .ToList();

        return Ok(response);
    }

    [HttpPatch("{accountId:long}/balance")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateBalance(
        long accountId,
        [FromBody] UpdateAccountBalanceRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new UpdateAccountBalanceCommand(
            userId,
            accountId,
            request.NewBalance);

        await _updateAccountBalanceHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
}