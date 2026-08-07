namespace MoneyFlow.Application.Accounts.GetAccountById;

public sealed record GetAccountByIdQuery(
    long UserId,
    long AccountId);