namespace MoneyFlow.API.Contracts.MonthlyPlans;

public sealed record UpdateCategoryLimitRequest(
    decimal LimitAmount);