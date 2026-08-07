namespace MoneyFlow.Application.Abstractions.Services;

public sealed record GeneratedAccessToken(
    string Token,
    DateTimeOffset ExpiresAt);