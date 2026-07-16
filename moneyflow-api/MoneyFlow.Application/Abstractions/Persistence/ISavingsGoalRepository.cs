using MoneyFlow.Domain.SavingsGoals;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface ISavingsGoalRepository
{
    Task<IReadOnlyList<SavingsGoal>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<SavingsGoal?> GetByIdAsync(
        long savingsGoalId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SavingsGoal savingsGoal,
        CancellationToken cancellationToken = default);
}