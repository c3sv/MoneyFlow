namespace MoneyFlow.Application.Users.ChangePassword;

public sealed record ChangePasswordCommand(
    long UserId,
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword);