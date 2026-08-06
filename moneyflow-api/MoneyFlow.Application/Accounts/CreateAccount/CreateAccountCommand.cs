using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.Application.Accounts.CreateAccount;

public sealed record CreateAccountCommand(
    long UserId,
    string Bank,
    string Nickname,
    AccountType Type,
    string Last4,
    decimal InitialBalance,
    string Currency);