namespace MoneyFlow.Application.Accounts.UpdateAccountBalance;

public sealed record UpdateAccountBalanceCommand(
    long UserId,
    long AccountId,
    decimal NewBalance);