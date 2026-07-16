using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.Categories;
using MoneyFlow.Domain.MonthlyPlans;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class MonthlyPlanCategoryLimitConfiguration
    : IEntityTypeConfiguration<MonthlyPlanCategoryLimit>
{
    public void Configure(
        EntityTypeBuilder<MonthlyPlanCategoryLimit> builder)
    {
        builder.ToTable("MonthlyPlanCategoryLimits");

        builder.HasKey(limit => limit.Id);

        builder.Property(limit => limit.Id)
            .ValueGeneratedOnAdd();

        builder.Property(limit => limit.LimitAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasIndex(limit => new
            {
                limit.MonthlyPlanId,
                limit.CategoryId
            })
            .IsUnique();

        builder.HasIndex(limit => limit.CategoryId);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(limit => limit.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}