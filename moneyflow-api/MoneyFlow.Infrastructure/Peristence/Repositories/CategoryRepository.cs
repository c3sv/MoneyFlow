using Microsoft.EntityFrameworkCore;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.Categories;
using MoneyFlow.Domain.Common;

namespace MoneyFlow.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly MoneyFlowDbContext _dbContext;

    public CategoryRepository(MoneyFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Category?> GetByIdAsync(
        long categoryId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories
            .FirstOrDefaultAsync(
                category => category.Id == categoryId,
                cancellationToken);
    }

    public Task<bool> ExistsByNameAndTypeAsync(
        long userId,
        string name,
        TransactionType type,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AnyAsync(
            category =>
                category.UserId == userId &&
                category.Name == name &&
                category.Type == type,
            cancellationToken);
    }

    public async Task AddAsync(
        Category category,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Categories.AddAsync(
            category,
            cancellationToken);
    }
}