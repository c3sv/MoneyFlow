namespace MoneyFlow.Application.Accounts.DeleteAccount;

public sealed record DeleteAccountCommand(
    long UserId,
    long AccountId);