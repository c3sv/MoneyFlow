namespace MoneyFlow.Application.Authentication.Logout;

public sealed record LogoutCommand(
    long UserId,
    string RefreshToken);