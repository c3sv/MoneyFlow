namespace MoneyFlow.Application.MonthlyPlans.DeleteCategoryLimit;

public sealed record DeleteCategoryLimitCommand(
    long UserId,
    long MonthlyPlanId,
    long CategoryId);