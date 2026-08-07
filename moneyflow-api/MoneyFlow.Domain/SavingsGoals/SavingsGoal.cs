using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.SavingsGoals;

public sealed class SavingsGoal
{
    private const int MaxTitleLength = 200;

    private SavingsGoal()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public string Title { get; private set; } = null!;

    public decimal TargetAmount { get; private set; }

    public decimal CurrentAmount { get; private set; }

    public DateTime? Deadline { get; private set; }

    public SavingsGoal(
        long userId,
        string title,
        decimal targetAmount,
        DateOnly? deadline,
        DateOnly currentDate)
    {
        if (userId <= 0)
        {
            throw new DomainException(
                "User id must be greater than zero.");
        }

        UserId = userId;
        CurrentAmount = 0;

        UpdateDetails(
            title,
            targetAmount,
            deadline,
            currentDate);
    }

    public void UpdateDetails(
        string title,
        decimal targetAmount,
        DateOnly? deadline,
        DateOnly currentDate)
    {
        var normalizedTitle = NormalizeTitle(title);

        if (targetAmount <= 0)
        {
            throw new DomainException(
                "Target amount must be greater than zero.");
        }

        if (targetAmount < CurrentAmount)
        {
            throw new DomainException(
                "Target amount cannot be lower than the current amount.");
        }

        ValidateDeadline(
            deadline,
            currentDate);

        Title = normalizedTitle;
        TargetAmount = targetAmount;
        Deadline = deadline?.ToDateTime(
            TimeOnly.MinValue);
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

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException(
                "Savings goal title is required.");
        }

        var normalizedTitle = title.Trim();

        if (normalizedTitle.Length > MaxTitleLength)
        {
            throw new DomainException(
                $"Savings goal title cannot exceed {MaxTitleLength} characters.");
        }

        return normalizedTitle;
    }

    private void ValidateDeadline(
        DateOnly? deadline,
        DateOnly currentDate)
    {
        if (!deadline.HasValue)
        {
            return;
        }

        var existingDeadline = Deadline.HasValue
            ? DateOnly.FromDateTime(Deadline.Value)
            : (DateOnly?)null;

        if (deadline.Value < currentDate &&
            deadline != existingDeadline)
        {
            throw new DomainException(
                "Savings goal deadline cannot be in the past.");
        }
    }
}