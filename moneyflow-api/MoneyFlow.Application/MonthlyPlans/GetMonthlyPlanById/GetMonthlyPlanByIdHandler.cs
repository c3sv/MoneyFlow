using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Application.MonthlyPlans.GetMonthlyPlans;

namespace MoneyFlow.Application.MonthlyPlans.GetMonthlyPlanById;

public sealed class GetMonthlyPlanByIdHandler
{
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;

    public GetMonthlyPlanByIdHandler(
        IMonthlyPlanRepository monthlyPlanRepository)
    {
        _monthlyPlanRepository = monthlyPlanRepository;
    }

    public async Task<MonthlyPlanResult> HandleAsync(
        GetMonthlyPlanByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var monthlyPlan =
            await _monthlyPlanRepository.GetByIdAsync(
                query.MonthlyPlanId,
                cancellationToken);

        if (monthlyPlan is null ||
            monthlyPlan.UserId != query.UserId)
        {
            throw new NotFoundException(
                $"Monthly plan with id {query.MonthlyPlanId} was not found.");
        }

        return new MonthlyPlanResult(
            monthlyPlan.Id,
            monthlyPlan.Month,
            monthlyPlan.Year,
            monthlyPlan.ExpectedIncome,
            monthlyPlan.TargetSavings,
            monthlyPlan.TotalSpendingLimit,
            monthlyPlan.Currency,
            monthlyPlan.CreatedAt,
            monthlyPlan.CategoryLimits
                .Select(limit => new CategoryLimitResult(
                    limit.Id,
                    limit.CategoryId,
                    limit.LimitAmount))
                .ToList());
    }
}