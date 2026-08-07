namespace MoneyFlow.Application.Abstractions.Services;

public sealed record GeneratedRefreshToken(
    string Token,
    string TokenHash,
    DateTimeOffset ExpiresAt);