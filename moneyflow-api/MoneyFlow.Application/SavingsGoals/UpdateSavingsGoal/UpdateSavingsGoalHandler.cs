using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Abstractions.Services;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.SavingsGoals.UpdateSavingsGoal;

public sealed class UpdateSavingsGoalHandler
{
    private readonly ISavingsGoalRepository _savingsGoalRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSavingsGoalHandler(
        ISavingsGoalRepository savingsGoalRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _savingsGoalRepository = savingsGoalRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateSavingsGoalCommand command,
        CancellationToken cancellationToken = default)
    {
        var savingsGoal =
            await _savingsGoalRepository.GetByIdAsync(
                command.SavingsGoalId,
                cancellationToken);

        if (savingsGoal is null ||
            savingsGoal.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Savings goal with id {command.SavingsGoalId} was not found.");
        }

        var currentDate = DateOnly.FromDateTime(
            _dateTimeProvider.UtcNow.UtcDateTime);

        savingsGoal.UpdateDetails(
            command.Title,
            command.TargetAmount,
            command.Deadline,
            currentDate);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}