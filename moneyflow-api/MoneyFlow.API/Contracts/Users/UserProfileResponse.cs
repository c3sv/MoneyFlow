namespace MoneyFlow.API.Contracts.Users;

public sealed record UserProfileResponse(
    long Id,
    string FirstName,
    string LastName,
    string Email,
    DateTimeOffset CreatedAt);