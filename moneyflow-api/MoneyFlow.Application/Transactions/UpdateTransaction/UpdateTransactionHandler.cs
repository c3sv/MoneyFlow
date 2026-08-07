using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.Application.Transactions.UpdateTransaction;

public sealed class UpdateTransactionHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTransactionHandler(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateTransactionCommand command,
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

        var currentAccount =
            await _accountRepository.GetByIdAsync(
                transaction.AccountId,
                cancellationToken);

        if (currentAccount is null ||
            currentAccount.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Account with id {transaction.AccountId} was not found.");
        }

        Account targetAccount;

        if (command.AccountId == transaction.AccountId)
        {
            targetAccount = currentAccount;
        }
        else
        {
            var requestedAccount =
                await _accountRepository.GetByIdAsync(
                    command.AccountId,
                    cancellationToken);

            if (requestedAccount is null ||
                requestedAccount.UserId != command.UserId)
            {
                throw new NotFoundException(
                    $"Account with id {command.AccountId} was not found.");
            }

            targetAccount = requestedAccount;
        }

        var category =
            await _categoryRepository.GetByIdAsync(
                command.CategoryId,
                cancellationToken);

        if (category is null ||
            category.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Category with id {command.CategoryId} was not found.");
        }

        if (category.Type != command.Type)
        {
            throw new BusinessRuleException(
                "Transaction type must match the category type.");
        }

        if (currentAccount.Id == targetAccount.Id)
        {
            currentAccount.ReplaceTransaction(
                transaction.Type,
                transaction.Amount,
                command.Type,
                command.Amount);
        }
        else
        {
            currentAccount.ReverseTransaction(
                transaction.Type,
                transaction.Amount);

            targetAccount.ApplyTransaction(
                command.Type,
                command.Amount);
        }

        transaction.UpdateDetails(
            command.AccountId,
            command.CategoryId,
            command.Amount,
            command.Description,
            command.Date,
            command.Type);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}