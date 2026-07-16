using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.Categories;
using MoneyFlow.Domain.Transactions;
using MoneyFlow.Domain.Users;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class TransactionConfiguration
    : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            date => date.ToDateTime(TimeOnly.MinValue),
            dateTime => DateOnly.FromDateTime(dateTime));
        
        builder.ToTable("Transactions");

        builder.HasKey(transaction => transaction.Id);

        builder.Property(transaction => transaction.Id)
            .ValueGeneratedOnAdd();

        builder.Property(transaction => transaction.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(transaction => transaction.Description)
            .HasMaxLength(500);
        
        builder.Property(transaction => transaction.Date)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(transaction => transaction.Type)
            .HasConversion<int>()
            .IsRequired();

        builder.HasIndex(transaction => transaction.UserId);

        builder.HasIndex(transaction => transaction.CategoryId);

        builder.HasIndex(transaction => new
        {
            transaction.UserId,
            transaction.Date
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(transaction => transaction.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(transaction => transaction.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}