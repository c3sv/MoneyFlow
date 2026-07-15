using Microsoft.EntityFrameworkCore;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Domain.Categories;
using MoneyFlow.Domain.FinancialInsights;
using MoneyFlow.Domain.MonthlyPlans;
using MoneyFlow.Domain.SavingsGoals;
using MoneyFlow.Domain.Transactions;
using MoneyFlow.Domain.Users;

namespace MoneyFlow.Infrastructure.Persistence;

public sealed class MoneyFlowDbContext
    : DbContext, IUnitOfWork
{
    public MoneyFlowDbContext(
        DbContextOptions<MoneyFlowDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Transaction> Transactions => Set<Transaction>();

    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();

    public DbSet<MonthlyPlan> MonthlyPlans => Set<MonthlyPlan>();

    public DbSet<MonthlyPlanCategoryLimit> MonthlyPlanCategoryLimits =>
        Set<MonthlyPlanCategoryLimit>();

    public DbSet<FinancialInsight> FinancialInsights =>
        Set<FinancialInsight>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(MoneyFlowDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}