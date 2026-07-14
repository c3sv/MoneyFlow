using MoneyFlow.Domain.Common;

namespace MoneyFlow.Domain.MonthlyPlans;

public sealed class MonthlyPlanCategoryLimit
{
    private MonthlyPlanCategoryLimit()
    {
    }

    public long Id { get; private set; }

    public long MonthlyPlanId { get; private set; }

    public long CategoryId { get; private set; }

    public decimal LimitAmount { get; private set; }

    internal MonthlyPlanCategoryLimit(
        long categoryId,
        decimal limitAmount)
    {
        if (categoryId <= 0)
        {
            throw new DomainException(
                "Category id must be greater than zero.");
        }

        if (limitAmount < 0)
        {
            throw new DomainException(
                "Category limit amount cannot be negative.");
        }

        CategoryId = categoryId;
        LimitAmount = limitAmount;
    }

    internal void ChangeAmount(decimal limitAmount)
    {
        if (limitAmount < 0)
        {
            throw new DomainException(
                "Category limit amount cannot be negative.");
        }

        LimitAmount = limitAmount;
    }
}