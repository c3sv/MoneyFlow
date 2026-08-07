using MoneyFlow.Domain.Users;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<User?> GetByIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByEmailExcludingIdAsync(
        string email,
        long excludedUserId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);
}