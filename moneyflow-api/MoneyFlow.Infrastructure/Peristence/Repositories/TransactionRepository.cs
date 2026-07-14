using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.Transactions;

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
}