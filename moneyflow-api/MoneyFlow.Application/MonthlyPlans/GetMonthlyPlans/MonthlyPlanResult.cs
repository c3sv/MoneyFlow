namespace MoneyFlow.Application.MonthlyPlans.GetMonthlyPlans;

public sealed record MonthlyPlanResult(
    long Id,
    int Month,
    int Year,
    decimal ExpectedIncome,
    decimal TargetSavings,
    decimal TotalSpendingLimit,
    string Currency,
    DateTimeOffset CreatedAt,
    IReadOnlyList<CategoryLimitResult> CategoryLimits);