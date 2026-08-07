namespace MoneyFlow.API.Contracts.SavingsGoals;

public sealed record UpdateSavingsGoalRequest(
    string Title,
    decimal TargetAmount,
    DateOnly? Deadline);