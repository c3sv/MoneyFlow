using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.MonthlyPlans;

public sealed class MonthlyPlan
{
    private readonly List<MonthlyPlanCategoryLimit> _categoryLimits = [];

    private MonthlyPlan()
    {
    }

    public long Id { get; private set; }

    public long UserId { get; private set; }

    public int Month { get; private set; }

    public int Year { get; private set; }

    public decimal ExpectedIncome { get; private set; }

    public decimal TargetSavings { get; private set; }

    public decimal TotalSpendingLimit { get; private set; }

    public string Currency { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<MonthlyPlanCategoryLimit> CategoryLimits =>
        _categoryLimits.AsReadOnly();

    public MonthlyPlan(
        long userId,
        int month,
        int year,
        decimal expectedIncome,
        decimal targetSavings,
        decimal totalSpendingLimit,
        string currency,
        DateTimeOffset createdAt)
    {
        if (userId <= 0)
        {
            throw new DomainException(
                "User id must be greater than zero.");
        }

        if (month is < 1 or > 12)
        {
            throw new DomainException(
                "Month must be between 1 and 12.");
        }

        if (year <= 0)
        {
            throw new DomainException(
                "Year must be greater than zero.");
        }

        if (expectedIncome < 0)
        {
            throw new DomainException(
                "Expected income cannot be negative.");
        }

        if (targetSavings < 0)
        {
            throw new DomainException(
                "Target savings cannot be negative.");
        }

        if (totalSpendingLimit < 0)
        {
            throw new DomainException(
                "Total spending limit cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required.");
        }

        UserId = userId;
        Month = month;
        Year = year;
        ExpectedIncome = expectedIncome;
        TargetSavings = targetSavings;
        TotalSpendingLimit = totalSpendingLimit;
        Currency = currency.Trim().ToUpperInvariant();
        CreatedAt = createdAt;
    }

    public void AddCategoryLimit(
        long categoryId,
        decimal limitAmount)
    {
        var categoryAlreadyExists = _categoryLimits.Any(
            limit => limit.CategoryId == categoryId);

        if (categoryAlreadyExists)
        {
            throw new DomainException(
                "The category already has a limit in this monthly plan.");
        }

        var categoryLimit = new MonthlyPlanCategoryLimit(
            categoryId,
            limitAmount);

        _categoryLimits.Add(categoryLimit);
    }

    public void ChangeCategoryLimit(
        long categoryId,
        decimal newLimitAmount)
    {
        var categoryLimit = _categoryLimits.FirstOrDefault(
            limit => limit.CategoryId == categoryId);

        if (categoryLimit is null)
        {
            throw new DomainException(
                "The category does not have a limit in this monthly plan.");
        }

        categoryLimit.ChangeAmount(newLimitAmount);
    }
}