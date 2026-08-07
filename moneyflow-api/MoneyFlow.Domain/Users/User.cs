using System.Net.Mail;
using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.Users;

public sealed class User
{
    private const int MaxFirstNameLength = 100;
    private const int MaxLastNameLength = 100;
    private const int MaxEmailLength = 255;
    private const int MaxPasswordHashLength = 255;

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
        PasswordHash = ValidateRequired(
            passwordHash,
            "Password hash",
            MaxPasswordHashLength);

        CreatedAt = createdAt;

        UpdateProfile(
            firstName,
            lastName,
            email);
    }

    public void UpdateProfile(
        string firstName,
        string lastName,
        string email)
    {
        FirstName = ValidateRequired(
            firstName,
            "First name",
            MaxFirstNameLength);

        LastName = ValidateRequired(
            lastName,
            "Last name",
            MaxLastNameLength);

        Email = NormalizeEmail(email);
    }

    public void ChangePasswordHash(string passwordHash)
    {
        PasswordHash = ValidateRequired(
            passwordHash,
            "Password hash",
            MaxPasswordHashLength);
    }

    private static string ValidateRequired(
        string value,
        string fieldName,
        int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException(
                $"{fieldName} is required.");
        }

        var normalizedValue = value.Trim();

        if (normalizedValue.Length > maxLength)
        {
            throw new DomainException(
                $"{fieldName} cannot exceed {maxLength} characters.");
        }

        return normalizedValue;
    }

    private static string NormalizeEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException(
                "Email is required.");
        }

        var normalizedEmail =
            email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > MaxEmailLength)
        {
            throw new DomainException(
                $"Email cannot exceed {MaxEmailLength} characters.");
        }

        if (!MailAddress.TryCreate(
                normalizedEmail,
                out var parsedEmail) ||
            parsedEmail.Address != normalizedEmail)
        {
            throw new DomainException(
                "Email format is invalid.");
        }

        return normalizedEmail;
    }
}