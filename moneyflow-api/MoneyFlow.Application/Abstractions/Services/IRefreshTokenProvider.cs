namespace MoneyFlow.Application.Abstractions.Services;

public interface IRefreshTokenProvider
{
    GeneratedRefreshToken Generate(DateTimeOffset createdAt);

    string ComputeHash(string token);
}