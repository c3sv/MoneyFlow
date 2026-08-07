using MoneyFlow.Domain.Common;

namespace MoneyFlow.Application.Transactions.GetTransactions;

public sealed record TransactionResult(
    long Id,
    long AccountId,
    long CategoryId,
    decimal Amount,
    string? Description,
    DateOnly Date,
    TransactionType Type);