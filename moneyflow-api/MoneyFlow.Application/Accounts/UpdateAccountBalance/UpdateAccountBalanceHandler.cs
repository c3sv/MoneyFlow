using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Accounts.UpdateAccountBalance;

public sealed class UpdateAccountBalanceHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountBalanceHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateAccountBalanceCommand command,
        CancellationToken cancellationToken = default)
    {
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

        account.UpdateBalance(command.NewBalance);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}