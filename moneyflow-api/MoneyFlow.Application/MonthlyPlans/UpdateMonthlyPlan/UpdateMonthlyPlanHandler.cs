using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.MonthlyPlans.UpdateMonthlyPlan;

public sealed class UpdateMonthlyPlanHandler
{
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMonthlyPlanHandler(
        IMonthlyPlanRepository monthlyPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _monthlyPlanRepository = monthlyPlanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateMonthlyPlanCommand command,
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

        monthlyPlan.UpdateDetails(
            command.ExpectedIncome,
            command.TargetSavings,
            command.TotalSpendingLimit);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}