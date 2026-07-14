using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyFlow.Domain.SavingsGoals;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence.Configurations;

public sealed class SavingsGoalConfiguration
    : IEntityTypeConfiguration<SavingsGoal>
{
    public void Configure(EntityTypeBuilder<SavingsGoal> builder)
    {
        builder.ToTable("SavingsGoals");

        builder.HasKey(goal => goal.Id);

        builder.Property(goal => goal.Id)
            .ValueGeneratedOnAdd();

        builder.Property(goal => goal.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(goal => goal.TargetAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(goal => goal.CurrentAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(goal => goal.Deadline)
            .HasColumnType("date");

        builder.HasIndex(goal => goal.UserId);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(goal => goal.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}