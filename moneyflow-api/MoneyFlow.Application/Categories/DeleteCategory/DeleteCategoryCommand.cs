namespace MoneyFlow.Application.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(
    long UserId,
    long CategoryId);