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
}