namespace MoneyFlow.Application.SavingsGoals.GetSavingsGoals;

public sealed record SavingsGoalResult(
    long Id,
    string Title,
    decimal TargetAmount,
    decimal CurrentAmount,
    DateOnly? Deadline);