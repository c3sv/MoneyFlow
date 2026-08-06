using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.Application.Accounts.CreateAccount;

public sealed class CreateAccountHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAccountHandler(
        IUserRepository userRepository,
        IAccountRepository accountRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _accountRepository = accountRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> HandleAsync(
        CreateAccountCommand command,
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

        var account = new Account(
            command.UserId,
            command.Bank,
            command.Nickname,
            command.Type,
            command.Last4,
            command.InitialBalance,
            command.Currency);

        await _accountRepository.AddAsync(account, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}