using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Users.UpdateUserProfile;

public sealed class UpdateUserProfileHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserProfileHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            command.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                $"User with id {command.UserId} was not found.");
        }

        user.UpdateProfile(
            command.FirstName,
            command.LastName,
            command.Email);

        var emailAlreadyExists =
            await _userRepository.ExistsByEmailExcludingIdAsync(
                user.Email,
                user.Id,
                cancellationToken);

        if (emailAlreadyExists)
        {
            throw new ConflictException(
                "A user with the same email already exists.");
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}