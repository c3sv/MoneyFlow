namespace MoneyFlow.Application.MonthlyPlans.UpdateCategoryLimit;

public sealed record UpdateCategoryLimitCommand(
    long UserId,
    long MonthlyPlanId,
    long CategoryId,
    decimal LimitAmount);