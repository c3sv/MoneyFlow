using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.Accounts;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration
    : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(account => account.Id);

        builder.Property(account => account.Id)
            .ValueGeneratedOnAdd();

        builder.Property(account => account.Bank)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(account => account.Nickname)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(account => account.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(account => account.Last4)
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(account => account.Balance)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(account => account.Currency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Property(account => account.UpdatedAt)
            .IsRequired();

        builder.HasIndex(account => account.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(account => account.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}