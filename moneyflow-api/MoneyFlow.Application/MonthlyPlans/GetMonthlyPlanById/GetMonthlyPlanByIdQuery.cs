namespace MoneyFlow.Application.MonthlyPlans.GetMonthlyPlanById;

public sealed record GetMonthlyPlanByIdQuery(
    long UserId,
    long MonthlyPlanId);