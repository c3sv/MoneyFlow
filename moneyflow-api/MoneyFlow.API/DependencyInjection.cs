using Microsoft.Extensions.DependencyInjection;
using MoneyFlow.Application.Authentication.Login;
using MoneyFlow.Application.Authentication.Register;
using MoneyFlow.Application.Categories.CreateCategory;
using MoneyFlow.Application.MonthlyPlans.AddCategoryLimit;
using MoneyFlow.Application.MonthlyPlans.CreateMonthlyPlan;
using MoneyFlow.Application.SavingsGoals.AddSavingsGoalProgress;
using MoneyFlow.Application.SavingsGoals.CreateSavingsGoal;
using MoneyFlow.Application.Transactions.CreateTransaction;
using MoneyFlow.Application.Categories.GetCategories;
using MoneyFlow.Application.Transactions.GetTransactions;
using MoneyFlow.Application.SavingsGoals.GetSavingsGoals;
using MoneyFlow.Application.MonthlyPlans.GetMonthlyPlans;

namespace MoneyFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<RegisterHandler>();
        services.AddScoped<LoginHandler>();

        services.AddScoped<CreateCategoryHandler>();
        services.AddScoped<CreateTransactionHandler>();

        services.AddScoped<CreateSavingsGoalHandler>();
        services.AddScoped<AddSavingsGoalProgressHandler>();

        services.AddScoped<CreateMonthlyPlanHandler>();
        services.AddScoped<AddCategoryLimitHandler>();
        
        services.AddScoped<GetCategoriesHandler>();
        services.AddScoped<GetTransactionsHandler>();
        services.AddScoped<GetSavingsGoalsHandler>();
        services.AddScoped<GetMonthlyPlansHandler>();
        
        return services;
    }
}