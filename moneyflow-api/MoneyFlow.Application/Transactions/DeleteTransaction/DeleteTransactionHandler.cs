using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Transactions.DeleteTransaction;

public sealed class DeleteTransactionHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTransactionHandler(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        var transaction =
            await _transactionRepository.GetByIdAsync(
                command.TransactionId,
                cancellationToken);

        if (transaction is null ||
            transaction.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Transaction with id {command.TransactionId} was not found.");
        }

        var account =
            await _accountRepository.GetByIdAsync(
                transaction.AccountId,
                cancellationToken);

        if (account is null ||
            account.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Account with id {transaction.AccountId} was not found.");
        }

        account.ReverseTransaction(
            transaction.Type,
            transaction.Amount);

        _transactionRepository.Remove(transaction);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}