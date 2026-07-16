using MoneyFlow.Domain.Common;

namespace MoneyFlow.API.Contracts.Transactions;

public sealed record CreateTransactionRequest(
    long CategoryId,
    decimal Amount,
    string? Description,
    DateOnly Date,
    TransactionType Type);