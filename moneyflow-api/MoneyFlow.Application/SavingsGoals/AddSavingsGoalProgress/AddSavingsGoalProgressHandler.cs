using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.SavingsGoals.AddSavingsGoalProgress;

public sealed class AddSavingsGoalProgressHandler
{
    private readonly ISavingsGoalRepository _savingsGoalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddSavingsGoalProgressHandler(
        ISavingsGoalRepository savingsGoalRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsGoalRepository = savingsGoalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        AddSavingsGoalProgressCommand command,
        CancellationToken cancellationToken = default)
    {
        var savingsGoal = await _savingsGoalRepository.GetByIdAsync(
            command.SavingsGoalId,
            cancellationToken);

        if (savingsGoal is null)
        {
            throw new NotFoundException(
                $"Savings goal with id {command.SavingsGoalId} was not found.");
        }

        if (savingsGoal.UserId != command.UserId)
        {
            throw new BusinessRuleException(
                "The savings goal does not belong to the user.");
        }

        savingsGoal.AddProgress(command.Amount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}