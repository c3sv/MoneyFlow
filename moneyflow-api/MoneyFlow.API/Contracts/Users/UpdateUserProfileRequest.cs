namespace MoneyFlow.API.Contracts.Users;

public sealed record UpdateUserProfileRequest(
    string FirstName,
    string LastName,
    string Email);