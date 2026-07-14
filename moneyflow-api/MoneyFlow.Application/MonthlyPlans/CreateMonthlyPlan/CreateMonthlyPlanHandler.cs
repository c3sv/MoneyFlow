using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.MonthlyPlans;

namespace MoneyFlow.Application.MonthlyPlans.CreateMonthlyPlan;

public sealed class CreateMonthlyPlanHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMonthlyPlanHandler(
        IUserRepository userRepository,
        IMonthlyPlanRepository monthlyPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _monthlyPlanRepository = monthlyPlanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> HandleAsync(
        CreateMonthlyPlanCommand command,
        CancellationToken cancellationToken = default)
    {
        var userExists = await _userRepository.ExistsByIdAsync(
            command.UserId,
            cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException(
                $"User with id {command.UserId} was not found.");
        }

        var monthlyPlan = new MonthlyPlan(
            command.UserId,
            command.Month,
            command.Year,
            command.ExpectedIncome,
            command.TargetSavings,
            command.TotalSpendingLimit,
            command.Currency,
            command.CreatedAt);

        var planAlreadyExists =
            await _monthlyPlanRepository.ExistsForMonthAsync(
                monthlyPlan.UserId,
                monthlyPlan.Month,
                monthlyPlan.Year,
                cancellationToken);

        if (planAlreadyExists)
        {
            throw new ConflictException(
                "A monthly plan already exists for the selected month and year.");
        }

        await _monthlyPlanRepository.AddAsync(
            monthlyPlan,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return monthlyPlan.Id;
    }
}