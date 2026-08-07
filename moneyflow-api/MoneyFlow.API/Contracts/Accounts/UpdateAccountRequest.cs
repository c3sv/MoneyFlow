using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.API.Contracts.Accounts;

public sealed record UpdateAccountRequest(
    string Bank,
    string Nickname,
    AccountType Type,
    string Last4);