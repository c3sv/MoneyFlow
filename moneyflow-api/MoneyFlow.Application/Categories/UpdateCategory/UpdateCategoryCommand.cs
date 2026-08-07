namespace MoneyFlow.Application.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(
    long UserId,
    long CategoryId,
    string Name,
    string? Icon);