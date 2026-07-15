using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.FinancialInsights;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class FinancialInsightConfiguration
    : IEntityTypeConfiguration<FinancialInsight>
{
    public void Configure(EntityTypeBuilder<FinancialInsight> builder)
    {
        builder.ToTable("FinancialInsights");

        builder.HasKey(insight => insight.Id);

        builder.Property(insight => insight.Id)
            .ValueGeneratedOnAdd();

        builder.Property(insight => insight.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(insight => insight.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(insight => insight.Type)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(insight => insight.Severity)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(insight => insight.CreatedAt)
            .HasConversion(
                createdAt => createdAt.UtcDateTime,
                value => new DateTimeOffset(
                    DateTime.SpecifyKind(value, DateTimeKind.Utc)))
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasIndex(insight => new
        {
            insight.UserId,
            insight.CreatedAt
        });

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(insight => insight.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}