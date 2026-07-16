namespace MoneyFlow.API.Contracts.SavingsGoals;

public sealed record CreateSavingsGoalRequest(
    string Title,
    decimal TargetAmount,
    DateOnly? Deadline);