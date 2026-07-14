using MoneyFlow.Domain.Common;

namespace MoneyFlow.Application.Transactions.CreateTransaction;

public sealed record CreateTransactionCommand(
    long UserId,
    long CategoryId,
    decimal Amount,
    string? Description,
    DateOnly Date,
    TransactionType Type);