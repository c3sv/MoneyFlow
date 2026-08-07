using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Accounts.UpdateAccount;

public sealed class UpdateAccountHandler
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAccountHandler(
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateAccountCommand command,
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

        account.UpdateDetails(
            command.Bank,
            command.Nickname,
            command.Type,
            command.Last4);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}