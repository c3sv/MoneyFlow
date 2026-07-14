namespace MoneyFlow.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(
        long userId,
        CancellationToken cancellationToken = default);
}