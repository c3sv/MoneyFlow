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

    Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default);
}