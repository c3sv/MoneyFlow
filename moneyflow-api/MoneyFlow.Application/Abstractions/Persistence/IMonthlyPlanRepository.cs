using MoneyFlow.Domain.MonthlyPlans;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface IMonthlyPlanRepository
{
    Task<MonthlyPlan?> GetByIdAsync(
        long monthlyPlanId,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsForMonthAsync(
        long userId,
        int month,
        int year,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        MonthlyPlan monthlyPlan,
        CancellationToken cancellationToken = default);
}