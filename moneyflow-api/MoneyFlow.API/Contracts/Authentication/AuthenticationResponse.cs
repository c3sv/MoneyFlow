namespace MoneyFlow.API.Contracts.Authentication;

public sealed record AuthenticationResponse(
    long UserId,
    string FirstName,
    string LastName,
    string Email,
    string Token);