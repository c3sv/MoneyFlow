using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.SavingsGoals;

public sealed class SavingsGoal
{
    private SavingsGoal()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public string Title { get; private set; } = null!;

    public decimal TargetAmount { get; private set; }

    public decimal CurrentAmount { get; private set; }

    public DateOnly? Deadline { get; private set; }

    public SavingsGoal(
        long userId,
        string title,
        decimal targetAmount,
        DateOnly? deadline)
    {
        if (userId <= 0)
        {
            throw new DomainException("User id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Savings goal title is required.");
        }

        if (targetAmount <= 0)
        {
            throw new DomainException(
                "Target amount must be greater than zero.");
        }

        UserId = userId;
        Title = title.Trim();
        TargetAmount = targetAmount;
        CurrentAmount = 0;
        Deadline = deadline;
    }

    public void AddProgress(decimal amount)
    {
        if (amount <= 0)
        {
            throw new DomainException(
                "Progress amount must be greater than zero.");
        }

        if (CurrentAmount + amount > TargetAmount)
        {
            throw new DomainException(
                "Current amount cannot exceed the target amount.");
        }

        CurrentAmount += amount;
    }
}