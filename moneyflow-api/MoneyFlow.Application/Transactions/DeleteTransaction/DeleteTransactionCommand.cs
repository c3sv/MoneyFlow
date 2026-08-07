namespace MoneyFlow.Application.Transactions.DeleteTransaction;

public sealed record DeleteTransactionCommand(
    long UserId,
    long TransactionId);