using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Users.GetCurrentUser;

public sealed class GetCurrentUserHandler
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileResult> HandleAsync(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(
            query.UserId,
            cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                $"User with id {query.UserId} was not found.");
        }

        return new UserProfileResult(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.CreatedAt);
    }
}