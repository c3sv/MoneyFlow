using MoneyFlow.Domain.Common;

namespace MoneyFlow.API.Contracts.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    TransactionType Type,
    string? Icon);