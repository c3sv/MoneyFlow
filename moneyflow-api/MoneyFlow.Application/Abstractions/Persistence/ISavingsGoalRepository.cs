using MoneyFlow.Domain.SavingsGoals;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface ISavingsGoalRepository
{
    Task<SavingsGoal?> GetByIdAsync(
        long savingsGoalId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SavingsGoal savingsGoal,
        CancellationToken cancellationToken = default);
}