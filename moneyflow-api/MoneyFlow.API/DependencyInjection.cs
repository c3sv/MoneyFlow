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
using MoneyFlow.Application.SavingsGoals.DeleteSavingsGoal;
using MoneyFlow.Application.SavingsGoals.GetSavingsGoalById;
using MoneyFlow.Application.SavingsGoals.UpdateSavingsGoal;
using MoneyFlow.Application.MonthlyPlans.DeleteCategoryLimit;
using MoneyFlow.Application.MonthlyPlans.DeleteMonthlyPlan;
using MoneyFlow.Application.MonthlyPlans.GetMonthlyPlanById;
using MoneyFlow.Application.MonthlyPlans.UpdateCategoryLimit;
using MoneyFlow.Application.MonthlyPlans.UpdateMonthlyPlan;
using MoneyFlow.Application.Users.ChangePassword;
using MoneyFlow.Application.Users.GetCurrentUser;
using MoneyFlow.Application.Users.UpdateUserProfile;

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
        
        services.AddScoped<GetCategoriesHandler>();
        services.AddScoped<UpdateCategoryHandler>();
        services.AddScoped<DeleteCategoryHandler>();
        
        services.AddScoped<GetTransactionsHandler>();
        services.AddScoped<GetTransactionByIdHandler>();
        services.AddScoped<UpdateTransactionHandler>();
        services.AddScoped<DeleteTransactionHandler>();

        services.AddScoped<CreateAccountHandler>();
        services.AddScoped<GetAccountsHandler>();
        services.AddScoped<GetAccountByIdHandler>();
        services.AddScoped<UpdateAccountHandler>();
        services.AddScoped<DeleteAccountHandler>();
        
        services.AddScoped<CreateSavingsGoalHandler>();
        services.AddScoped<GetSavingsGoalsHandler>();
        services.AddScoped<GetSavingsGoalByIdHandler>();
        services.AddScoped<UpdateSavingsGoalHandler>();
        services.AddScoped<AddSavingsGoalProgressHandler>();
        services.AddScoped<DeleteSavingsGoalHandler>();
        
        services.AddScoped<CreateMonthlyPlanHandler>();
        services.AddScoped<GetMonthlyPlansHandler>();
        services.AddScoped<GetMonthlyPlanByIdHandler>();
        services.AddScoped<UpdateMonthlyPlanHandler>();
        services.AddScoped<DeleteMonthlyPlanHandler>();
        
        services.AddScoped<AddCategoryLimitHandler>();
        services.AddScoped<UpdateCategoryLimitHandler>();
        services.AddScoped<DeleteCategoryLimitHandler>();
        
        services.AddScoped<GetCurrentUserHandler>();
        services.AddScoped<UpdateUserProfileHandler>();
        services.AddScoped<ChangePasswordHandler>();
        
        return services;
    }
}