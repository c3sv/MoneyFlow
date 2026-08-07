namespace MoneyFlow.Application.Authentication;

public sealed record AuthenticationResult(
    long UserId,
    string FirstName,
    string LastName,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt);