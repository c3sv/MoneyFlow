namespace MoneyFlow.Application.MonthlyPlans.DeleteMonthlyPlan;

public sealed record DeleteMonthlyPlanCommand(
    long UserId,
    long MonthlyPlanId);