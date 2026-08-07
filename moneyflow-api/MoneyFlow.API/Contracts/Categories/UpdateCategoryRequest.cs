namespace MoneyFlow.API.Contracts.Categories;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Icon);