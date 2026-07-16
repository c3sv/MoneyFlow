namespace MoneyFlow.API.Contracts.MonthlyPlans;

public sealed record AddCategoryLimitRequest(
    long CategoryId,
    decimal LimitAmount);