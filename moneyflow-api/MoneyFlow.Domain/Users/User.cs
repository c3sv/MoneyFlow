using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Users;

public sealed class User
{
    private User()
    {
    }

    public long Id { get; private set; }

    public string FirstName { get; private set; } = null!;

    public string LastName { get; private set; } = null!;

    public string Email { get; private set; } = null!;

    public string PasswordHash { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public User(
        string firstName,
        string lastName,
        string email,
        string passwordHash,
        DateTimeOffset createdAt)
    {
        FirstName = ValidateRequired(firstName, "First name");
        LastName = ValidateRequired(lastName, "Last name");
        Email = NormalizeEmail(email);
        PasswordHash = ValidateRequired(passwordHash, "Password hash");
        CreatedAt = createdAt;
    }

    private static string ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required.");
        }

        return email.Trim().ToLowerInvariant();
    }
}