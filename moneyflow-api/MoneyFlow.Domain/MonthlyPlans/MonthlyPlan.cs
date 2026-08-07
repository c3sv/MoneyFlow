using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.MonthlyPlans;

public sealed class MonthlyPlan
{
    private const string SupportedCurrency = "PEN";

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

        UserId = userId;
        Month = month;
        Year = year;
        Currency = NormalizeCurrency(currency);
        CreatedAt = createdAt;

        UpdateDetails(
            expectedIncome,
            targetSavings,
            totalSpendingLimit);
    }

    public void UpdateDetails(
        decimal expectedIncome,
        decimal targetSavings,
        decimal totalSpendingLimit)
    {
        ValidateFinancialAmounts(
            expectedIncome,
            targetSavings,
            totalSpendingLimit);

        var allocatedCategoryLimits =
            _categoryLimits.Sum(limit => limit.LimitAmount);

        if (allocatedCategoryLimits > totalSpendingLimit)
        {
            throw new DomainException(
                "Total spending limit cannot be lower than the sum of category limits.");
        }

        ExpectedIncome = expectedIncome;
        TargetSavings = targetSavings;
        TotalSpendingLimit = totalSpendingLimit;
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

        EnsureCategoryLimitFits(
            currentLimitAmount: 0,
            newLimitAmount: limitAmount);

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

        EnsureCategoryLimitFits(
            categoryLimit.LimitAmount,
            newLimitAmount);

        categoryLimit.ChangeAmount(newLimitAmount);
    }

    public void RemoveCategoryLimit(long categoryId)
    {
        var categoryLimit = _categoryLimits.FirstOrDefault(
            limit => limit.CategoryId == categoryId);

        if (categoryLimit is null)
        {
            throw new DomainException(
                "The category does not have a limit in this monthly plan.");
        }

        _categoryLimits.Remove(categoryLimit);
    }

    private void EnsureCategoryLimitFits(
        decimal currentLimitAmount,
        decimal newLimitAmount)
    {
        if (newLimitAmount < 0)
        {
            throw new DomainException(
                "Category limit amount cannot be negative.");
        }

        var allocatedWithoutCurrent =
            _categoryLimits.Sum(limit => limit.LimitAmount) -
            currentLimitAmount;

        if (allocatedWithoutCurrent + newLimitAmount >
            TotalSpendingLimit)
        {
            throw new DomainException(
                "The sum of category limits cannot exceed the total spending limit.");
        }
    }

    private static void ValidateFinancialAmounts(
        decimal expectedIncome,
        decimal targetSavings,
        decimal totalSpendingLimit)
    {
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

        if (targetSavings + totalSpendingLimit >
            expectedIncome)
        {
            throw new DomainException(
                "Target savings and total spending limit cannot exceed expected income.");
        }
    }

    private static string NormalizeCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException(
                "Currency is required.");
        }

        var normalizedCurrency =
            currency.Trim().ToUpperInvariant();

        if (normalizedCurrency != SupportedCurrency)
        {
            throw new DomainException(
                $"Only {SupportedCurrency} currency is supported.");
        }

        return normalizedCurrency;
    }
}