namespace MoneyFlow.Application.SavingsGoals.AddSavingsGoalProgress;

public sealed record AddSavingsGoalProgressCommand(
    long UserId,
    long SavingsGoalId,
    decimal Amount);