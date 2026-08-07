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
    Task<bool> ExistsByCategoryIdAsync(
        long categoryId,
        CancellationToken cancellationToken = default);
    Task<Transaction?> GetByIdAsync(
        long transactionId,
        CancellationToken cancellationToken = default);
    void Remove(Transaction transaction);
    Task<bool> ExistsByAccountIdAsync(
        long accountId,
        CancellationToken cancellationToken = default);
}