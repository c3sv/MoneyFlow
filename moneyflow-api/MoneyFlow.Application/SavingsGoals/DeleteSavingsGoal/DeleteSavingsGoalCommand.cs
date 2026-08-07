namespace MoneyFlow.Application.SavingsGoals.DeleteSavingsGoal;

public sealed record DeleteSavingsGoalCommand(
    long UserId,
    long SavingsGoalId);