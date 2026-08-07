namespace MoneyFlow.API.Contracts.MonthlyPlans;

public sealed record UpdateMonthlyPlanRequest(
    decimal ExpectedIncome,
    decimal TargetSavings,
    decimal TotalSpendingLimit);