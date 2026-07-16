using MoneyFlow.Application.Abstractions.Persistence;

namespace MoneyFlow.Application.Categories.GetCategories;

public sealed class GetCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesHandler(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryResult>> HandleAsync(
        GetCategoriesQuery query,
        CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetByUserIdAsync(
            query.UserId,
            cancellationToken);

        return categories
            .Select(category => new CategoryResult(
                category.Id,
                category.Name,
                category.Type,
                category.Icon))
            .ToList();
    }
}