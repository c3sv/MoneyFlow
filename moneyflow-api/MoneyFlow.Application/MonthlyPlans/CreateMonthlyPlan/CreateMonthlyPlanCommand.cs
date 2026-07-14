namespace MoneyFlow.Application.MonthlyPlans.CreateMonthlyPlan;

public sealed record CreateMonthlyPlanCommand(
    long UserId,
    int Month,
    int Year,
    decimal ExpectedIncome,
    decimal TargetSavings,
    decimal TotalSpendingLimit,
    string Currency,
    DateTimeOffset CreatedAt);