namespace MoneyFlow.API.Contracts.MonthlyPlans;

public sealed record CategoryLimitResponse(
    long Id,
    long CategoryId,
    decimal LimitAmount);