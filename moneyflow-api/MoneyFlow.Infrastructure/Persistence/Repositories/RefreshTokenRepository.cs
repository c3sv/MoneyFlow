using Microsoft.EntityFrameworkCore;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.RefreshTokens;

namespace MoneyFlow.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MoneyFlowDbContext _dbContext;

    public RefreshTokenRepository(MoneyFlowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.RefreshTokens
            .FirstOrDefaultAsync(
                refreshToken => refreshToken.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
        long userId,
        DateTimeOffset now,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.RefreshTokens
            .Where(refreshToken =>
                refreshToken.UserId == userId &&
                refreshToken.RevokedAt == null &&
                refreshToken.ExpiresAt > now)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.RefreshTokens.AddAsync(
            refreshToken,
            cancellationToken);
    }
}