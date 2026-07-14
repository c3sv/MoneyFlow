namespace MoneyFlow.Application.MonthlyPlans.AddCategoryLimit;

public sealed record AddCategoryLimitCommand(
    long UserId,
    long MonthlyPlanId,
    long CategoryId,
    decimal LimitAmount);