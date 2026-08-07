using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.Application.Abstractions.Persistence;

public interface IAccountRepository
{
    Task<IReadOnlyList<Account>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default);

    Task<Account?> GetByIdAsync(
        long accountId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default);
    
    void Remove(Account account);
}