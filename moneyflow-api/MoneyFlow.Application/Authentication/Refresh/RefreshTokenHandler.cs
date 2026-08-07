using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.RefreshTokens;

namespace MoneyFlow.Application.Authentication.Refresh;

public sealed class RefreshTokenHandler
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITokenProvider tokenProvider,
        IRefreshTokenProvider refreshTokenProvider,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tokenProvider = tokenProvider;
        _refreshTokenProvider = refreshTokenProvider;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> HandleAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            throw new ValidationException("Refresh token is required.");
        }

        var tokenHash = _refreshTokenProvider.ComputeHash(
            command.RefreshToken.Trim());

        var currentRefreshToken =
            await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

        if (currentRefreshToken is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        var now = _dateTimeProvider.UtcNow;

        if (currentRefreshToken.RevokedAt is not null)
        {
            await RevokeActiveSessionsAfterReuseAsync(
                currentRefreshToken,
                now,
                cancellationToken);

            throw new UnauthorizedException(
                "Refresh token has already been used or revoked.");
        }

        if (!currentRefreshToken.IsActive(now))
        {
            throw new UnauthorizedException("Refresh token has expired.");
        }

        var user = await _userRepository.GetByIdAsync(
            currentRefreshToken.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        var generatedRefreshToken = _refreshTokenProvider.Generate(now);

        await _unitOfWork.ExecuteInTransactionAsync(
            async transactionCancellationToken =>
            {
                currentRefreshToken.Revoke(
                    now,
                    generatedRefreshToken.TokenHash);

                var newRefreshToken = new RefreshToken(
                    user.Id,
                    generatedRefreshToken.TokenHash,
                    generatedRefreshToken.ExpiresAt,
                    now);

                await _refreshTokenRepository.AddAsync(
                    newRefreshToken,
                    transactionCancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    transactionCancellationToken);
            },
            cancellationToken);

        var accessToken = _tokenProvider.Generate(user);

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

    private async Task RevokeActiveSessionsAfterReuseAsync(
        RefreshToken reusedRefreshToken,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        if (reusedRefreshToken.ReplacedByTokenHash is null)
        {
            return;
        }

        var activeRefreshTokens =
            await _refreshTokenRepository.GetActiveByUserIdAsync(
                reusedRefreshToken.UserId,
                now,
                cancellationToken);

        foreach (var activeRefreshToken in activeRefreshTokens)
        {
            activeRefreshToken.Revoke(now);
        }

        if (activeRefreshTokens.Count > 0)
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}