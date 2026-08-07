using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.RefreshTokens;

namespace MoneyFlow.Application.Authentication.Login;

public sealed class LoginHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public LoginHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider,
        IRefreshTokenProvider refreshTokenProvider,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _refreshTokenProvider = refreshTokenProvider;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
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

        var now = _dateTimeProvider.UtcNow;
        var accessToken = _tokenProvider.Generate(user);
        var generatedRefreshToken = _refreshTokenProvider.Generate(now);

        var refreshToken = new RefreshToken(
            user.Id,
            generatedRefreshToken.TokenHash,
            generatedRefreshToken.ExpiresAt,
            now);

        await _refreshTokenRepository.AddAsync(
            refreshToken,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            accessToken.Token,
            accessToken.ExpiresAt,
            generatedRefreshToken.Token,
            generatedRefreshToken.ExpiresAt);
    }
}