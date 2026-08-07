using MoneyFlow.Domain.Categories;
using MoneyFlow.Domain.Common;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(
        long categoryId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAndTypeAsync(
        long userId,
        string name,
        TransactionType type,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAndTypeExcludingIdAsync(
        long userId,
        string name,
        TransactionType type,
        long excludedCategoryId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<Category> categories,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Category>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
    
    void Remove(Category category);
}