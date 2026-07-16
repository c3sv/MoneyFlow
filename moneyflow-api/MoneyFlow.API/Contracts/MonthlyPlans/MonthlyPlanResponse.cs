namespace MoneyFlow.API.Contracts.MonthlyPlans;

public sealed record MonthlyPlanResponse(
    long Id,
    int Month,
    int Year,
    decimal ExpectedIncome,
    decimal TargetSavings,
    decimal TotalSpendingLimit,
    string Currency,
    DateTimeOffset CreatedAt,
    IReadOnlyList<CategoryLimitResponse> CategoryLimits);