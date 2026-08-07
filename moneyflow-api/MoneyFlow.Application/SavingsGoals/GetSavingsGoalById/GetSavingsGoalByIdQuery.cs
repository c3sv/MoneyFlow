namespace MoneyFlow.Application.SavingsGoals.GetSavingsGoalById;

public sealed record GetSavingsGoalByIdQuery(
    long UserId,
    long SavingsGoalId);