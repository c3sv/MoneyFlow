using MoneyFlow.Application.Abstractions.Persistence;

namespace MoneyFlow.Application.Transactions.GetTransactions;

public sealed class GetTransactionsHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsHandler(
        ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IReadOnlyList<TransactionResult>> HandleAsync(
        GetTransactionsQuery query,
        CancellationToken cancellationToken = default)
    {
        var transactions =
            await _transactionRepository.GetByUserIdAsync(
                query.UserId,
                cancellationToken);

        return transactions
            .Select(transaction => new TransactionResult(
                transaction.Id,
                transaction.CategoryId,
                transaction.Amount,
                transaction.Description,
                DateOnly.FromDateTime(transaction.Date),
                transaction.Type))
            .ToList();
    }
}