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
using MoneyFlow.Application.Accounts.CreateAccount;
using MoneyFlow.Application.Accounts.GetAccounts;
using MoneyFlow.Application.Categories.UpdateCategory;
using MoneyFlow.Application.Categories.DeleteCategory;
using MoneyFlow.Application.Transactions.DeleteTransaction;
using MoneyFlow.Application.Transactions.GetTransactionById;
using MoneyFlow.Application.Transactions.UpdateTransaction;
using MoneyFlow.Application.Accounts.DeleteAccount;
using MoneyFlow.Application.Accounts.GetAccountById;
using MoneyFlow.Application.Accounts.UpdateAccount;


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
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();
        
        
        services.AddScoped<GetSavingsGoalsHandler>();
        services.AddScoped<GetMonthlyPlansHandler>();
        
        services.AddScoped<GetTransactionsHandler>();
        services.AddScoped<GetTransactionByIdHandler>();
        services.AddScoped<UpdateTransactionHandler>();
        services.AddScoped<DeleteTransactionHandler>();

        services.AddScoped<CreateAccountHandler>();
        services.AddScoped<GetAccountsHandler>();
        services.AddScoped<GetAccountByIdHandler>();
        services.AddScoped<UpdateAccountHandler>();
        services.AddScoped<DeleteAccountHandler>();
        return services;
    }
}