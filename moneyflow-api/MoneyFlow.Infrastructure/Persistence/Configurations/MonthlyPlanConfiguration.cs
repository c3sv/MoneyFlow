using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.MonthlyPlans;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class MonthlyPlanConfiguration
    : IEntityTypeConfiguration<MonthlyPlan>
{
    public void Configure(EntityTypeBuilder<MonthlyPlan> builder)
    {
        builder.ToTable("MonthlyPlans");

        builder.HasKey(plan => plan.Id);

        builder.Property(plan => plan.Id)
            .ValueGeneratedOnAdd();

        builder.Property(plan => plan.Month)
            .IsRequired();

        builder.Property(plan => plan.Year)
            .IsRequired();

        builder.Property(plan => plan.ExpectedIncome)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(plan => plan.TargetSavings)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(plan => plan.TotalSpendingLimit)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(plan => plan.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(plan => plan.CreatedAt)
            .HasConversion(
                createdAt => createdAt.UtcDateTime,
                value => new DateTimeOffset(
                    DateTime.SpecifyKind(value, DateTimeKind.Utc)))
            .HasColumnType("datetime(6)")
            .IsRequired();

        builder.HasIndex(plan => new
        {
            plan.UserId,
            plan.Month,
            plan.Year
        })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(plan => plan.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(plan => plan.CategoryLimits)
            .WithOne()
            .HasForeignKey(limit => limit.MonthlyPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(plan => plan.CategoryLimits)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}