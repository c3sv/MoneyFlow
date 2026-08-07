using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Accounts;
using MoneyFlow.Application.Accounts.CreateAccount;
using MoneyFlow.Application.Accounts.DeleteAccount;
using MoneyFlow.Application.Accounts.GetAccountById;
using MoneyFlow.Application.Accounts.GetAccounts;
using MoneyFlow.Application.Accounts.UpdateAccount;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/accounts")]
public sealed class AccountsController : ControllerBase
{
    private readonly CreateAccountHandler _createAccountHandler;
    private readonly GetAccountsHandler _getAccountsHandler;
    private readonly GetAccountByIdHandler _getAccountByIdHandler;
    private readonly UpdateAccountHandler _updateAccountHandler;
    private readonly DeleteAccountHandler _deleteAccountHandler;

    public AccountsController(
        CreateAccountHandler createAccountHandler,
        GetAccountsHandler getAccountsHandler,
        GetAccountByIdHandler getAccountByIdHandler,
        UpdateAccountHandler updateAccountHandler,
        DeleteAccountHandler deleteAccountHandler)
    {
        _createAccountHandler = createAccountHandler;
        _getAccountsHandler = getAccountsHandler;
        _getAccountByIdHandler = getAccountByIdHandler;
        _updateAccountHandler = updateAccountHandler;
        _deleteAccountHandler = deleteAccountHandler;
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

        return StatusCode(
            StatusCodes.Status201Created,
            response);
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
            .Select(ToResponse)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{accountId:long}")]
    [ProducesResponseType(
        typeof(AccountResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountResponse>> GetById(
        long accountId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetAccountByIdQuery(
            userId,
            accountId);

        var result = await _getAccountByIdHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(ToResponse(result));
    }

    [HttpPut("{accountId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long accountId,
        [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new UpdateAccountCommand(
            userId,
            accountId,
            request.Bank,
            request.Nickname,
            request.Type,
            request.Last4);

        await _updateAccountHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }

    [HttpDelete("{accountId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(
        long accountId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new DeleteAccountCommand(
            userId,
            accountId);

        await _deleteAccountHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }

    private static AccountResponse ToResponse(
        MoneyFlow.Application.Accounts.GetAccounts.AccountResult account)
    {
        return new AccountResponse(
            account.Id,
            account.Bank,
            account.Nickname,
            account.Type,
            account.Last4,
            account.Balance,
            account.Currency,
            account.UpdatedAt);
    }
}