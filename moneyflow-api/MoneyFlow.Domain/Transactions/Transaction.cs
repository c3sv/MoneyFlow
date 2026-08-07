using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Transactions;

public sealed class Transaction
{
    private Transaction()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public long AccountId { get; private set; }

    public long CategoryId { get; private set; }

    public decimal Amount { get; private set; }

    public string? Description { get; private set; }

    public DateTime Date { get; private set; }

    public TransactionType Type { get; private set; }

    public Transaction(
        long userId,
        long accountId,
        long categoryId,
        decimal amount,
        string? description,
        DateOnly date,
        TransactionType type)
    {
        if (userId <= 0)
        {
            throw new DomainException(
                "User id must be greater than zero.");
        }

        if (accountId <= 0)
        {
            throw new DomainException(
                "Account id must be greater than zero.");
        }

        if (categoryId <= 0)
        {
            throw new DomainException(
                "Category id must be greater than zero.");
        }

        if (amount <= 0)
        {
            throw new DomainException(
                "Transaction amount must be greater than zero.");
        }

        if (!Enum.IsDefined(type))
        {
            throw new DomainException(
                "Transaction type is invalid.");
        }

        var normalizedDescription = NormalizeDescription(description);

        if (normalizedDescription?.Length > 500)
        {
            throw new DomainException(
                "Transaction description cannot exceed 500 characters.");
        }

        UserId = userId;
        AccountId = accountId;
        CategoryId = categoryId;
        Amount = amount;
        Description = normalizedDescription;
        Date = date.ToDateTime(TimeOnly.MinValue);
        Type = type;
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }
}