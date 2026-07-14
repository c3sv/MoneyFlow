using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id)
            .ValueGeneratedOnAdd();

        builder.Property(user => user.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(user => user.CreatedAt)
            .HasConversion(
                createdAt => createdAt.UtcDateTime,
                value => new DateTimeOffset(
                    DateTime.SpecifyKind(value, DateTimeKind.Utc)))
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();
    }
}