using MoneyFlow.Domain.Common;

namespace MoneyFlow.API.Contracts.Categories;

public sealed record CategoryResponse(
    long Id,
    string Name,
    TransactionType Type,
    string? Icon);