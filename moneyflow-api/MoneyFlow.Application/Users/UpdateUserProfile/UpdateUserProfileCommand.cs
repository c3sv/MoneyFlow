namespace MoneyFlow.Application.Users.UpdateUserProfile;

public sealed record UpdateUserProfileCommand(
    long UserId,
    string FirstName,
    string LastName,
    string Email);