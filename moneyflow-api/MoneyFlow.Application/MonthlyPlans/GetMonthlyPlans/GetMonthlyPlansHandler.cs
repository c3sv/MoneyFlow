using MoneyFlow.Application.Abstractions.Persistence;

namespace MoneyFlow.Application.MonthlyPlans.GetMonthlyPlans;

public sealed class GetMonthlyPlansHandler
{
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;

    public GetMonthlyPlansHandler(
        IMonthlyPlanRepository monthlyPlanRepository)
    {
        _monthlyPlanRepository = monthlyPlanRepository;
    }

    public async Task<IReadOnlyList<MonthlyPlanResult>> HandleAsync(
        GetMonthlyPlansQuery query,
        CancellationToken cancellationToken = default)
    {
        var monthlyPlans =
            await _monthlyPlanRepository.GetByUserIdAsync(
                query.UserId,
                cancellationToken);

        return monthlyPlans
            .Select(plan => new MonthlyPlanResult(
                plan.Id,
                plan.Month,
                plan.Year,
                plan.ExpectedIncome,
                plan.TargetSavings,
                plan.TotalSpendingLimit,
                plan.Currency,
                plan.CreatedAt,
                plan.CategoryLimits
                    .Select(limit => new CategoryLimitResult(
                        limit.Id,
                        limit.CategoryId,
                        limit.LimitAmount))
                    .ToList()))
            .ToList();
    }
}