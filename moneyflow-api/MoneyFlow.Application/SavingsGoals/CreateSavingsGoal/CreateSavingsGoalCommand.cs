namespace MoneyFlow.Application.SavingsGoals.CreateSavingsGoal;

public sealed record CreateSavingsGoalCommand(
    long UserId,
    string Title,
    decimal TargetAmount,
    DateOnly? Deadline);