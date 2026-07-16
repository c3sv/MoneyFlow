using MoneyFlow.Application.Abstractions.Persistence;

namespace MoneyFlow.Application.SavingsGoals.GetSavingsGoals;

public sealed class GetSavingsGoalsHandler
{
    private readonly ISavingsGoalRepository _savingsGoalRepository;

    public GetSavingsGoalsHandler(
        ISavingsGoalRepository savingsGoalRepository)
    {
        _savingsGoalRepository = savingsGoalRepository;
    }

    public async Task<IReadOnlyList<SavingsGoalResult>> HandleAsync(
        GetSavingsGoalsQuery query,
        CancellationToken cancellationToken = default)
    {
        var savingsGoals =
            await _savingsGoalRepository.GetByUserIdAsync(
                query.UserId,
                cancellationToken);

        return savingsGoals
            .Select(goal => new SavingsGoalResult(
                goal.Id,
                goal.Title,
                goal.TargetAmount,
                goal.CurrentAmount,
                goal.Deadline.HasValue
                    ? DateOnly.FromDateTime(goal.Deadline.Value)
                    : null))
            .ToList();
    }
}