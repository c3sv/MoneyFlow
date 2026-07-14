using MoneyFlow.Domain.Transactions;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface ITransactionRepository
{
    Task AddAsync(
        Transaction transaction,
        CancellationToken cancellationToken = default);
}