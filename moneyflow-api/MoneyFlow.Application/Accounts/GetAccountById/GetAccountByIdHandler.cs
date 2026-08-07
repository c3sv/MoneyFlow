using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Accounts.GetAccounts;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Accounts.GetAccountById;

public sealed class GetAccountByIdHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdHandler(
        IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountResult> HandleAsync(
        GetAccountByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(
            query.AccountId,
            cancellationToken);

        if (account is null ||
            account.UserId != query.UserId)
        {
            throw new NotFoundException(
                $"Account with id {query.AccountId} was not found.");
        }

        return new AccountResult(
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