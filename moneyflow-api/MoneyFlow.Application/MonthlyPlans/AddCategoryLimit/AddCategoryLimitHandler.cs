using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.MonthlyPlans.AddCategoryLimit;

public sealed class AddCategoryLimitHandler
{
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddCategoryLimitHandler(
        IMonthlyPlanRepository monthlyPlanRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _monthlyPlanRepository = monthlyPlanRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        AddCategoryLimitCommand command,
        CancellationToken cancellationToken = default)
    {
        var monthlyPlan = await _monthlyPlanRepository.GetByIdAsync(
            command.MonthlyPlanId,
            cancellationToken);

        if (monthlyPlan is null)
        {
            throw new NotFoundException(
                $"Monthly plan with id {command.MonthlyPlanId} was not found.");
        }

        if (monthlyPlan.UserId != command.UserId)
        {
            throw new BusinessRuleException(
                "The monthly plan does not belong to the user.");
        }

        var category = await _categoryRepository.GetByIdAsync(
            command.CategoryId,
            cancellationToken);

        if (category is null)
        {
            throw new NotFoundException(
                $"Category with id {command.CategoryId} was not found.");
        }

        if (category.UserId != command.UserId)
        {
            throw new BusinessRuleException(
                "The category does not belong to the user.");
        }

        monthlyPlan.AddCategoryLimit(
            category.Id,
            command.LimitAmount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}