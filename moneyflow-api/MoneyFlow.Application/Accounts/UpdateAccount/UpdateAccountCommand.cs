using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.Application.Accounts.UpdateAccount;

public sealed record UpdateAccountCommand(
    long UserId,
    long AccountId,
    string Bank,
    string Nickname,
    AccountType Type,
    string Last4);