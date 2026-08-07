using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.RefreshTokens;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(refreshToken => refreshToken.Id);

        builder.Property(refreshToken => refreshToken.Id)
            .ValueGeneratedOnAdd();

        builder.Property(refreshToken => refreshToken.TokenHash)
            .HasMaxLength(64)
            .IsRequired();

        builder.Property(refreshToken => refreshToken.ExpiresAt)
            .HasConversion(
                expiresAt => expiresAt.UtcDateTime,
                value => new DateTimeOffset(
                    DateTime.SpecifyKind(value, DateTimeKind.Utc)))
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(refreshToken => refreshToken.CreatedAt)
            .HasConversion(
                createdAt => createdAt.UtcDateTime,
                value => new DateTimeOffset(
                    DateTime.SpecifyKind(value, DateTimeKind.Utc)))
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.Property(refreshToken => refreshToken.RevokedAt)
            .HasConversion(
                revokedAt => revokedAt.HasValue
                    ? revokedAt.Value.UtcDateTime
                    : (DateTime?)null,
                value => value.HasValue
                    ? new DateTimeOffset(
                        DateTime.SpecifyKind(value.Value, DateTimeKind.Utc))
                    : null)
            .HasColumnType("datetime(6)");

        builder.Property(refreshToken => refreshToken.ReplacedByTokenHash)
            .HasMaxLength(64);

        builder.HasIndex(refreshToken => refreshToken.TokenHash)
            .IsUnique();

        builder.HasIndex(refreshToken => refreshToken.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(refreshToken => refreshToken.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}