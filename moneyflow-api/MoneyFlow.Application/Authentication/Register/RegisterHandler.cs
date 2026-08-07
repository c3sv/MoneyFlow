using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.Categories;
using MoneyFlow.Domain.RefreshTokens;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Application.Authentication.Register;

public sealed class RegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IRefreshTokenProvider _refreshTokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider,
        IRefreshTokenProvider refreshTokenProvider,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
        _refreshTokenProvider = refreshTokenProvider;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthenticationResult> HandleAsync(
        RegisterCommand command,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Password))
        {
            throw new ValidationException("Password is required.");
        }

        var passwordHash = _passwordHasher.Hash(command.Password);
        var createdAt = _dateTimeProvider.UtcNow;

        var user = new User(
            command.FirstName,
            command.LastName,
            command.Email,
            passwordHash,
            createdAt);

        var existingUser = await _userRepository.GetByEmailAsync(
            user.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "A user with the same email already exists.");
        }

        GeneratedRefreshToken? generatedRefreshToken = null;

        await _unitOfWork.ExecuteInTransactionAsync(
            async transactionCancellationToken =>
            {
                await _userRepository.AddAsync(
                    user,
                    transactionCancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    transactionCancellationToken);

                var defaultCategories =
                    DefaultCategoryFactory.CreateForUser(user.Id);

                await _categoryRepository.AddRangeAsync(
                    defaultCategories,
                    transactionCancellationToken);

                generatedRefreshToken =
                    _refreshTokenProvider.Generate(createdAt);

                var refreshToken = new RefreshToken(
                    user.Id,
                    generatedRefreshToken.TokenHash,
                    generatedRefreshToken.ExpiresAt,
                    createdAt);

                await _refreshTokenRepository.AddAsync(
                    refreshToken,
                    transactionCancellationToken);

                await _unitOfWork.SaveChangesAsync(
                    transactionCancellationToken);
            },
            cancellationToken);

        var issuedRefreshToken = generatedRefreshToken
            ?? throw new InvalidOperationException(
                "The refresh token could not be generated.");

        var accessToken = _tokenProvider.Generate(user);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            accessToken.Token,
            accessToken.ExpiresAt,
            issuedRefreshToken.Token,
            issuedRefreshToken.ExpiresAt);
    }
}