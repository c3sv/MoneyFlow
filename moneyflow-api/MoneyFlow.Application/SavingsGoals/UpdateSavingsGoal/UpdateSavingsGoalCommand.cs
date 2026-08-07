namespace MoneyFlow.Application.SavingsGoals.UpdateSavingsGoal;

public sealed record UpdateSavingsGoalCommand(
    long UserId,
    long SavingsGoalId,
    string Title,
    decimal TargetAmount,
    DateOnly? Deadline);