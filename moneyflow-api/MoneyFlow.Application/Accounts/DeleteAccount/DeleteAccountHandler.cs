using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Accounts.DeleteAccount;

public sealed class DeleteAccountHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAccountHandler(
        IAccountRepository accountRepository,
        ITransactionRepository transactionRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteAccountCommand command,
        CancellationToken cancellationToken = default)
    {
        var account = await _accountRepository.GetByIdAsync(
            command.AccountId,
            cancellationToken);

        if (account is null ||
            account.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Account with id {command.AccountId} was not found.");
        }

        var hasTransactions =
            await _transactionRepository.ExistsByAccountIdAsync(
                account.Id,
                cancellationToken);

        if (hasTransactions)
        {
            throw new ConflictException(
                "The account cannot be deleted because it is used by one or more transactions.");
        }

        _accountRepository.Remove(account);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}