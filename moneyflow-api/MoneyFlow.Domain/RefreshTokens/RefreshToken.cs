using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.RefreshTokens;

public sealed class RefreshToken
{
    private const int TokenHashLength = 64;

    private RefreshToken()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public string TokenHash { get; private set; } = null!;

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset? RevokedAt { get; private set; }

    public string? ReplacedByTokenHash { get; private set; }

    public RefreshToken(
        long userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt)
    {
        if (userId <= 0)
        {
            throw new DomainException("User id must be greater than zero.");
        }

        if (expiresAt <= createdAt)
        {
            throw new DomainException(
                "Refresh token expiration must be after its creation date.");
        }

        UserId = userId;
        TokenHash = ValidateTokenHash(tokenHash);
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    public bool IsActive(DateTimeOffset now)
    {
        return RevokedAt is null && ExpiresAt > now;
    }

    public void Revoke(
        DateTimeOffset revokedAt,
        string? replacedByTokenHash = null)
    {
        if (RevokedAt is not null)
        {
            return;
        }

        if (revokedAt < CreatedAt)
        {
            throw new DomainException(
                "Refresh token revocation cannot be before its creation date.");
        }

        RevokedAt = revokedAt;
        ReplacedByTokenHash = replacedByTokenHash is null
            ? null
            : ValidateTokenHash(replacedByTokenHash);
    }

    private static string ValidateTokenHash(string tokenHash)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new DomainException("Refresh token hash is required.");
        }

        var normalizedHash = tokenHash.Trim();

        if (normalizedHash.Length != TokenHashLength)
        {
            throw new DomainException(
                $"Refresh token hash must contain {TokenHashLength} characters.");
        }

        return normalizedHash;
    }
}