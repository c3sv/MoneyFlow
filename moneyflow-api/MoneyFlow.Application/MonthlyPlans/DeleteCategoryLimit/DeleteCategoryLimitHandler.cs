using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.MonthlyPlans.DeleteCategoryLimit;

public sealed class DeleteCategoryLimitHandler
{
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryLimitHandler(
        IMonthlyPlanRepository monthlyPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _monthlyPlanRepository = monthlyPlanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteCategoryLimitCommand command,
        CancellationToken cancellationToken = default)
    {
        var monthlyPlan =
            await _monthlyPlanRepository.GetByIdAsync(
                command.MonthlyPlanId,
                cancellationToken);

        if (monthlyPlan is null ||
            monthlyPlan.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Monthly plan with id {command.MonthlyPlanId} was not found.");
        }

        monthlyPlan.RemoveCategoryLimit(
            command.CategoryId);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}