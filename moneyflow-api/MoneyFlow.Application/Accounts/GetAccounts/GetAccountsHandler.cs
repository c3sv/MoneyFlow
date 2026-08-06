using MoneyFlow.Application.Abstractions.Persistence;

namespace MoneyFlow.Application.Accounts.GetAccounts;

public sealed class GetAccountsHandler
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<IReadOnlyList<AccountResult>> HandleAsync(
        GetAccountsQuery query,
        CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.GetByUserIdAsync(
            query.UserId,
            cancellationToken);

        return accounts
            .Select(account => new AccountResult(
                account.Id,
                account.Bank,
                account.Nickname,
                account.Type,
                account.Last4,
                account.Balance,
                account.Currency,
                account.UpdatedAt))
            .ToList();
    }
}