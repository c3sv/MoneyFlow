using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Application.SavingsGoals.GetSavingsGoals;

namespace MoneyFlow.Application.SavingsGoals.GetSavingsGoalById;

public sealed class GetSavingsGoalByIdHandler
{
    private readonly ISavingsGoalRepository _savingsGoalRepository;

    public GetSavingsGoalByIdHandler(
        ISavingsGoalRepository savingsGoalRepository)
    {
        _savingsGoalRepository = savingsGoalRepository;
    }

    public async Task<SavingsGoalResult> HandleAsync(
        GetSavingsGoalByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var savingsGoal =
            await _savingsGoalRepository.GetByIdAsync(
                query.SavingsGoalId,
                cancellationToken);

        if (savingsGoal is null ||
            savingsGoal.UserId != query.UserId)
        {
            throw new NotFoundException(
                $"Savings goal with id {query.SavingsGoalId} was not found.");
        }

        return new SavingsGoalResult(
            savingsGoal.Id,
            savingsGoal.Title,
            savingsGoal.TargetAmount,
            savingsGoal.CurrentAmount,
            savingsGoal.Deadline.HasValue
                ? DateOnly.FromDateTime(savingsGoal.Deadline.Value)
                : null);
    }
}