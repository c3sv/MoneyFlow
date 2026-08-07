using MoneyFlow.Domain.Common;

namespace MoneyFlow.API.Contracts.Transactions;

public sealed record TransactionResponse(
    long Id,
    long AccountId,
    long CategoryId,
    decimal Amount,
    string? Description,
    DateOnly Date,
    TransactionType Type);