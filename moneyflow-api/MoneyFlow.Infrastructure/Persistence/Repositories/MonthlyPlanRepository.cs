using Microsoft.EntityFrameworkCore;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.MonthlyPlans;

namespace MoneyFlow.Infrastructure.Persistence.Repositories;

public sealed class MonthlyPlanRepository : IMonthlyPlanRepository
{
    private readonly MoneyFlowDbContext _dbContext;

    public MonthlyPlanRepository(MoneyFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<MonthlyPlan?> GetByIdAsync(
        long monthlyPlanId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.MonthlyPlans
            .Include(plan => plan.CategoryLimits)
            .FirstOrDefaultAsync(
                plan => plan.Id == monthlyPlanId,
                cancellationToken);
    }

    public Task<bool> ExistsForMonthAsync(
        long userId,
        int month,
        int year,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.MonthlyPlans.AnyAsync(
            plan =>
                plan.UserId == userId &&
                plan.Month == month &&
                plan.Year == year,
            cancellationToken);
    }

    public async Task AddAsync(
        MonthlyPlan monthlyPlan,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.MonthlyPlans.AddAsync(
            monthlyPlan,
            cancellationToken);
    }
    
    public async Task<IReadOnlyList<MonthlyPlan>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.MonthlyPlans
            .AsNoTracking()
            .Include(plan => plan.CategoryLimits)
            .Where(plan => plan.UserId == userId)
            .OrderByDescending(plan => plan.Year)
            .ThenByDescending(plan => plan.Month)
            .ToListAsync(cancellationToken);
    }
}