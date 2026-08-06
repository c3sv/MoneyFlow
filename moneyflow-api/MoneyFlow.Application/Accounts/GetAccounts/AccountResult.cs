using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.Application.Accounts.GetAccounts;

public sealed record AccountResult(
    long Id,
    string Bank,
    string Nickname,
    AccountType Type,
    string Last4,
    decimal Balance,
    string Currency,
    DateTime UpdatedAt);