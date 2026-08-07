namespace MoneyFlow.Application.Users;

public sealed record UserProfileResult(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    DateTimeOffset CreatedAt);