using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.FinancialInsights;

public sealed class FinancialInsight
{
    private FinancialInsight()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public string Type { get; private set; } = null!;

    public InsightSeverity Severity { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public FinancialInsight(
        long userId,
        string title,
        string description,
        string type,
        InsightSeverity severity,
        DateTimeOffset createdAt)
    {
        if (userId <= 0)
        {
            throw new DomainException(
                "User id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException(
                "Financial insight title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException(
                "Financial insight description is required.");
        }

        if (string.IsNullOrWhiteSpace(type))
        {
            throw new DomainException(
                "Financial insight type is required.");
        }

        if (!Enum.IsDefined(severity))
        {
            throw new DomainException(
                "Financial insight severity is invalid.");
        }

        UserId = userId;
        Title = title.Trim();
        Description = description.Trim();
        Type = type.Trim();
        Severity = severity;
        CreatedAt = createdAt;
    }
}