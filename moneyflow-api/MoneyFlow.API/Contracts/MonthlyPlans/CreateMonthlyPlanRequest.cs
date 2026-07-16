namespace MoneyFlow.API.Contracts.MonthlyPlans;

public sealed record CreateMonthlyPlanRequest(
    int Month,
    int Year,
    decimal ExpectedIncome,
    decimal TargetSavings,
    decimal TotalSpendingLimit,
    string Currency);