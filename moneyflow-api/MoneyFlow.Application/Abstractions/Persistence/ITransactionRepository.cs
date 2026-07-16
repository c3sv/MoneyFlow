using MoneyFlow.Domain.Transactions;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface ITransactionRepository
{
    Task<IReadOnlyList<Transaction>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);
}