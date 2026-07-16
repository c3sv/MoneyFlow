namespace MoneyFlow.Application.MonthlyPlans.GetMonthlyPlans;

public sealed record CategoryLimitResult(
    long Id,
    long CategoryId,
    decimal LimitAmount);