using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Authentication.Login;

public sealed class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public LoginHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<AuthenticationResult> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) ||
            string.IsNullOrWhiteSpace(command.Password))
        {
            throw new ValidationException(
                "Email and password are required.");
        }

        var normalizedEmail = command.Email
            .Trim()
            .ToLowerInvariant();

        var user = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (user is null ||
            !_passwordHasher.Verify(command.Password, user.PasswordHash))
        {
            throw new ValidationException(
                "Invalid email or password.");
        }

        var token = _tokenProvider.Generate(user);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            token);
    }
}