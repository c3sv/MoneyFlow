using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Application.Transactions.GetTransactions;

namespace MoneyFlow.Application.Transactions.GetTransactionById;

public sealed class GetTransactionByIdHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdHandler(
        ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionResult> HandleAsync(
        GetTransactionByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var transaction =
            await _transactionRepository.GetByIdAsync(
                query.TransactionId,
                cancellationToken);

        if (transaction is null ||
            transaction.UserId != query.UserId)
        {
            throw new NotFoundException(
                $"Transaction with id {query.TransactionId} was not found.");
        }

        return new TransactionResult(
            transaction.Id,
            transaction.AccountId,
            transaction.CategoryId,
            transaction.Amount,
            transaction.Description,
            DateOnly.FromDateTime(transaction.Date),
            transaction.Type);
    }
}