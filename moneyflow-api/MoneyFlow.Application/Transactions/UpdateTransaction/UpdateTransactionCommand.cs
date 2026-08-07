using MoneyFlow.Domain.Common;

namespace MoneyFlow.Application.Transactions.UpdateTransaction;

public sealed record UpdateTransactionCommand(
    long UserId,
    long TransactionId,
    long AccountId,
    long CategoryId,
    decimal Amount,
    string? Description,
    DateOnly Date,
    TransactionType Type);