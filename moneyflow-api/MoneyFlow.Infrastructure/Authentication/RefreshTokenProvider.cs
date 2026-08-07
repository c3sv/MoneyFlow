using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MoneyFlow.Application.Abstractions.Services;

namespace MoneyFlow.Infrastructure.Authentication;

public sealed class RefreshTokenProvider : IRefreshTokenProvider
{
    private const int TokenSizeInBytes = 64;

    private readonly JwtSettings _settings;

    public RefreshTokenProvider(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public GeneratedRefreshToken Generate(DateTimeOffset createdAt)
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);
        var token = Base64UrlEncoder.Encode(tokenBytes);
        var tokenHash = ComputeHash(token);
        var expiresAt = createdAt.AddDays(
            _settings.RefreshTokenExpirationDays);

        return new GeneratedRefreshToken(token, tokenHash, expiresAt);
    }

    public string ComputeHash(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException(
                "Refresh token is required.",
                nameof(token));
        }

        var tokenBytes = Encoding.UTF8.GetBytes(token);
        var hashBytes = SHA256.HashData(tokenBytes);

        return Convert.ToHexString(hashBytes);
    }
}