using MoneyFlow.Domain.Common;

namespace MoneyFlow.Application.Categories.GetCategories;

public sealed record CategoryResult(
    long Id,
    string Name,
    TransactionType Type,
    string? Icon);