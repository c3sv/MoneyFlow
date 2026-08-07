using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Categories;

public sealed class Category
{
    private const int MaxNameLength = 100;
    private const int MaxIconLength = 100;

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
            throw new DomainException(
                "User id must be greater than zero.");
        }

        if (!Enum.IsDefined(type))
        {
            throw new DomainException(
                "Category type is invalid.");
        }

        UserId = userId;
        Type = type;

        UpdateDetails(name, icon);
    }

    public void UpdateDetails(
        string name,
        string? icon)
    {
        Name = NormalizeName(name);
        Icon = NormalizeIcon(icon);
    }

    private static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException(
                "Category name is required.");
        }

        var normalizedName = name.Trim();

        if (normalizedName.Length > MaxNameLength)
        {
            throw new DomainException(
                $"Category name cannot exceed {MaxNameLength} characters.");
        }

        return normalizedName;
    }

    private static string? NormalizeIcon(string? icon)
    {
        if (string.IsNullOrWhiteSpace(icon))
        {
            return null;
        }

        var normalizedIcon = icon.Trim();

        if (normalizedIcon.Length > MaxIconLength)
        {
            throw new DomainException(
                $"Category icon cannot exceed {MaxIconLength} characters.");
        }

        return normalizedIcon;
    }
}