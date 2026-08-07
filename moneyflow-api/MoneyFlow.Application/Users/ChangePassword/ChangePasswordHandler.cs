using System.Text;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Users.ChangePassword;

public sealed class ChangePasswordHandler
{
    private const int MinPasswordLength = 8;
    private const int MaxPasswordBytes = 72;

    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        ChangePasswordCommand command,
        CancellationToken cancellationToken = default)
    {
        ValidateRequiredFields(command);

        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                $"User with id {command.UserId} was not found.");
        }

        if (!_passwordHasher.Verify(
                command.CurrentPassword,
                user.PasswordHash))
        {
            throw new ValidationException(
                "Current password is incorrect.");
        }

        ValidateNewPassword(command.NewPassword);

        if (command.NewPassword != command.ConfirmNewPassword)
        {
            throw new ValidationException(
                "New password and confirmation do not match.");
        }

        if (_passwordHasher.Verify(
                command.NewPassword,
                user.PasswordHash))
        {
            throw new ValidationException(
                "New password must be different from the current password.");
        }

        var newPasswordHash = _passwordHasher.Hash(
            command.NewPassword);

        user.ChangePasswordHash(newPasswordHash);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }

    private static void ValidateRequiredFields(
        ChangePasswordCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.CurrentPassword) ||
            string.IsNullOrWhiteSpace(command.NewPassword) ||
            string.IsNullOrWhiteSpace(command.ConfirmNewPassword))
        {
            throw new ValidationException(
                "Current password, new password and confirmation are required.");
        }
    }

    private static void ValidateNewPassword(string password)
    {
        if (password.Length < MinPasswordLength)
        {
            throw new ValidationException(
                $"New password must contain at least {MinPasswordLength} characters.");
        }

        if (Encoding.UTF8.GetByteCount(password) > MaxPasswordBytes)
        {
            throw new ValidationException(
                $"New password cannot exceed {MaxPasswordBytes} UTF-8 bytes.");
        }

        if (!password.Any(char.IsUpper) ||
            !password.Any(char.IsLower) ||
            !password.Any(char.IsDigit))
        {
            throw new ValidationException(
                "New password must include an uppercase letter, a lowercase letter and a number.");
        }
    }
}