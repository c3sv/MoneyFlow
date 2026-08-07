using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.MonthlyPlans.DeleteMonthlyPlan;

public sealed class DeleteMonthlyPlanHandler
{
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMonthlyPlanHandler(
        IMonthlyPlanRepository monthlyPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _monthlyPlanRepository = monthlyPlanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteMonthlyPlanCommand command,
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

        _monthlyPlanRepository.Remove(monthlyPlan);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}