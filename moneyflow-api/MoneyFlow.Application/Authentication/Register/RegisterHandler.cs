using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Application.Authentication.Register;

public sealed class RegisterHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
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

        var user = new User(
            command.FirstName,
            command.LastName,
            command.Email,
            passwordHash,
            _dateTimeProvider.UtcNow);

        var existingUser = await _userRepository.GetByEmailAsync(
            user.Email,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new ConflictException(
                "A user with the same email already exists.");
        }

        await _userRepository.AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenProvider.Generate(user);

        return new AuthenticationResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            token);
    }
}