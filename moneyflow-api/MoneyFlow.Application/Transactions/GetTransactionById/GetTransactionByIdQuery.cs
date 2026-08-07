namespace MoneyFlow.Application.Transactions.GetTransactionById;

public sealed record GetTransactionByIdQuery(
    long UserId,
    long TransactionId);