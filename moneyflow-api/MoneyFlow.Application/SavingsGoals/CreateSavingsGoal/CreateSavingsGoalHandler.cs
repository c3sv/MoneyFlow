using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.SavingsGoals;

namespace MoneyFlow.Application.SavingsGoals.CreateSavingsGoal;

public sealed class CreateSavingsGoalHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ISavingsGoalRepository _savingsGoalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSavingsGoalHandler(
        IUserRepository userRepository,
        ISavingsGoalRepository savingsGoalRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _savingsGoalRepository = savingsGoalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> HandleAsync(
        CreateSavingsGoalCommand command,
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

        var savingsGoal = new SavingsGoal(
            command.UserId,
            command.Title,
            command.TargetAmount,
            command.Deadline);

        await _savingsGoalRepository.AddAsync(
            savingsGoal,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return savingsGoal.Id;
    }
}