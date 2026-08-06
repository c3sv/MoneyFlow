using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.API.Contracts.Accounts;

public sealed record CreateAccountRequest(
    string Bank,
    string Nickname,
    AccountType Type,
    string Last4,
    decimal InitialBalance,
    string Currency);