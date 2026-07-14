using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Transactions;

public sealed class Transaction
{
    private Transaction()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public long CategoryId { get; private set; }

    public decimal Amount { get; private set; }

    public string? Description { get; private set; }

    public DateOnly Date { get; private set; }

    public TransactionType Type { get; private set; }

    public Transaction(
        long userId,
        long categoryId,
        decimal amount,
        string? description,
        DateOnly date,
        TransactionType type)
    {
        if (userId <= 0)
        {
            throw new DomainException("User id must be greater than zero.");
        }

        if (categoryId <= 0)
        {
            throw new DomainException("Category id must be greater than zero.");
        }

        if (amount <= 0)
        {
            throw new DomainException("Transaction amount must be greater than zero.");
        }

        if (!Enum.IsDefined(type))
        {
            throw new DomainException("Transaction type is invalid.");
        }

        UserId = userId;
        CategoryId = categoryId;
        Amount = amount;
        Description = NormalizeDescription(description);
        Date = date;
        Type = type;
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }
}