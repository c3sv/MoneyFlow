using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Infrastructure.Authentication;
using MoneyFlow.Infrastructure.Persistence;
using MoneyFlow.Infrastructure.Persistence.Repositories;
using MoneyFlow.Infrastructure.Services;

namespace MoneyFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddAuthenticationServices(services, configuration);

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static void AddPersistence(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<MoneyFlowDbContext>(options =>
            options.UseMySQL(connectionString));

        services.AddScoped<IUnitOfWork>(serviceProvider =>
            serviceProvider.GetRequiredService<MoneyFlowDbContext>());

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<ITransactionRepository, TransactionRepository>();

        services.AddScoped<ISavingsGoalRepository, SavingsGoalRepository>();

        services.AddScoped<IMonthlyPlanRepository, MonthlyPlanRepository>();
    }

    private static void AddAuthenticationServices(
        IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(
                settings =>
                    !string.IsNullOrWhiteSpace(settings.Issuer),
                "JWT issuer is required.")
            .Validate(
                settings =>
                    !string.IsNullOrWhiteSpace(settings.Audience),
                "JWT audience is required.")
            .Validate(
                settings =>
                    Encoding.UTF8.GetByteCount(settings.SecretKey) >= 32,
                "JWT secret key must contain at least 32 bytes.")
            .Validate(
                settings =>
                    settings.ExpirationMinutes > 0,
                "JWT expiration must be greater than zero.")
            .ValidateOnStart();

        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddSingleton<ITokenProvider, JwtTokenProvider>();
    }
}