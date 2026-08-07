using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.SavingsGoals.DeleteSavingsGoal;

public sealed class DeleteSavingsGoalHandler
{
    private readonly ISavingsGoalRepository _savingsGoalRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSavingsGoalHandler(
        ISavingsGoalRepository savingsGoalRepository,
        IUnitOfWork unitOfWork)
    {
        _savingsGoalRepository = savingsGoalRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteSavingsGoalCommand command,
        CancellationToken cancellationToken = default)
    {
        var savingsGoal =
            await _savingsGoalRepository.GetByIdAsync(
                command.SavingsGoalId,
                cancellationToken);

        if (savingsGoal is null ||
            savingsGoal.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Savings goal with id {command.SavingsGoalId} was not found.");
        }

        _savingsGoalRepository.Remove(savingsGoal);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}