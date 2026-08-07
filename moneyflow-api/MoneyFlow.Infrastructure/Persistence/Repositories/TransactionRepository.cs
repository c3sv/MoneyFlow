using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.Transactions;
using Microsoft.EntityFrameworkCore;
namespace MoneyFlow.Infrastructure.Persistence.Repositories;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly MoneyFlowDbContext _dbContext;

    public TransactionRepository(MoneyFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Transactions.AddAsync(
            transaction,
            cancellationToken);
    }
    public async Task<IReadOnlyList<Transaction>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Transactions
            .AsNoTracking()
            .Where(transaction => transaction.UserId == userId)
            .OrderByDescending(transaction => transaction.Date)
            .ThenByDescending(transaction => transaction.Id)
            .ToListAsync(cancellationToken);
    }
    public Task<bool> ExistsByCategoryIdAsync(
        long categoryId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Transactions.AnyAsync(
            transaction => transaction.CategoryId == categoryId,
            cancellationToken);
    }
    public Task<Transaction?> GetByIdAsync(
        long transactionId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Transactions
            .FirstOrDefaultAsync(
                transaction => transaction.Id == transactionId,
                cancellationToken);
    }

    public void Remove(Transaction transaction)
    {
        _dbContext.Transactions.Remove(transaction);
    }
}