using Microsoft.EntityFrameworkCore;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.SavingsGoals;

namespace MoneyFlow.Infrastructure.Persistence.Repositories;

public sealed class SavingsGoalRepository : ISavingsGoalRepository
{
    private readonly MoneyFlowDbContext _dbContext;

    public SavingsGoalRepository(MoneyFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<SavingsGoal?> GetByIdAsync(
        long savingsGoalId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.SavingsGoals
            .FirstOrDefaultAsync(
                goal => goal.Id == savingsGoalId,
                cancellationToken);
    }

    public async Task AddAsync(
        SavingsGoal savingsGoal,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SavingsGoals.AddAsync(
            savingsGoal,
            cancellationToken);
    }
}