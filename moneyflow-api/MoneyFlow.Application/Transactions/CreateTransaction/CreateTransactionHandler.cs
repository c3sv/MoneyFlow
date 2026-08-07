using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.Transactions;

namespace MoneyFlow.Application.Transactions.CreateTransaction;

public sealed class CreateTransactionHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTransactionHandler(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> HandleAsync(
        CreateTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(
            command.UserId,
            cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException(
                $"User with id {command.UserId} was not found.");
        }

        var account = await _accountRepository.GetByIdAsync(
            command.AccountId,
            cancellationToken);

        if (account is null)
        {
            throw new NotFoundException(
                $"Account with id {command.AccountId} was not found.");
        }

        if (account.UserId != command.UserId)
        {
            throw new BusinessRuleException(
                "The account does not belong to the user.");
        }

        var category = await _categoryRepository.GetByIdAsync(
            command.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new NotFoundException(
                $"Category with id {command.CategoryId} was not found.");
        }

        if (category.UserId != command.UserId)
        {
            throw new BusinessRuleException(
                "The category does not belong to the user.");
        }

        if (category.Type != command.Type)
        {
            throw new BusinessRuleException(
                "Transaction type must match the category type.");
        }

        account.ApplyTransaction(
            command.Type,
            command.Amount);

        var transaction = new Transaction(
            command.UserId,
            command.AccountId,
            command.CategoryId,
            command.Amount,
            command.Description,
            command.Date,
            command.Type);

        await _transactionRepository.AddAsync(
            transaction,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return transaction.Id;
    }
}