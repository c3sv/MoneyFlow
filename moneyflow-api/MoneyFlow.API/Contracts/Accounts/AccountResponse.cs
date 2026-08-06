using MoneyFlow.Domain.Accounts;
 
namespace MoneyFlow.API.Contracts.Accounts;
 
public sealed record AccountResponse(
    long Id,
    string Bank,
    string Nickname,
    AccountType Type,
    string Last4,
    decimal Balance,
    string Currency,
    DateTime UpdatedAt);