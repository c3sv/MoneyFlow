using MoneyFlow.Domain.Common;

namespace MoneyFlow.Application.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    long UserId,
    string Name,
    TransactionType Type,
    string? Icon);