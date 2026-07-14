using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Categories;

public sealed class Category
{
    private Category()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public string Name { get; private set; } = null!;

    public TransactionType Type { get; private set; }

    public string? Icon { get; private set; }

    public Category(
        long userId,
        string name,
        TransactionType type,
        string? icon)
    {
        if (userId <= 0)
        {
            throw new DomainException("User id must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Category name is required.");
        }

        if (!Enum.IsDefined(type))
        {
            throw new DomainException("Category type is invalid.");
        }

        UserId = userId;
        Name = name.Trim();
        Type = type;
        Icon = NormalizeIcon(icon);
    }

    private static string? NormalizeIcon(string? icon)
    {
        return string.IsNullOrWhiteSpace(icon)
            ? null
            : icon.Trim();
    }
}