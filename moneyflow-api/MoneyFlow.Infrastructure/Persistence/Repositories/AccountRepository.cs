using Microsoft.EntityFrameworkCore;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.Accounts;

namespace MoneyFlow.Infrastructure.Persistence.Repositories;

public sealed class AccountRepository : IAccountRepository
{
    private readonly MoneyFlowDbContext _dbContext;

    public AccountRepository(MoneyFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Account?> GetByIdAsync(
        long accountId,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Accounts
            .FirstOrDefaultAsync(
                account => account.Id == accountId,
                cancellationToken);
    }

    public async Task AddAsync(
        Account account,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.Accounts.AddAsync(account, cancellationToken);
    }

    public async Task<IReadOnlyList<Account>> GetByUserIdAsync(
        long userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Accounts
            .AsNoTracking()
            .Where(account => account.UserId == userId)
            .OrderBy(account => account.Bank)
            .ThenBy(account => account.Nickname)
            .ToListAsync(cancellationToken);
    }
    public void Remove(Account account)
    {
        _dbContext.Accounts.Remove(account);
    }
}