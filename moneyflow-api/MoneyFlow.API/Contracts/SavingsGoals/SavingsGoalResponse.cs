namespace MoneyFlow.API.Contracts.SavingsGoals;

public sealed record SavingsGoalResponse(
    long Id,
    string Title,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateOnly? Deadline);