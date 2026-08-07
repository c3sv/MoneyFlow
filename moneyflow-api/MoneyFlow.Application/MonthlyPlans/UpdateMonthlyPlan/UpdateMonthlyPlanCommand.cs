namespace MoneyFlow.Application.MonthlyPlans.UpdateMonthlyPlan;

public sealed record UpdateMonthlyPlanCommand(
    long UserId,
    long MonthlyPlanId,
    decimal ExpectedIncome,
    decimal TargetSavings,
    decimal TotalSpendingLimit);