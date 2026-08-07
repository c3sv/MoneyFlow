using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMonthlyPlanRepository _monthlyPlanRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(
        ICategoryRepository categoryRepository,
        ITransactionRepository transactionRepository,
        IMonthlyPlanRepository monthlyPlanRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _transactionRepository = transactionRepository;
        _monthlyPlanRepository = monthlyPlanRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        DeleteCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(
            command.CategoryId,
            cancellationToken);

        if (category is null ||
            category.UserId != command.UserId)
        {
            throw new NotFoundException(
                $"Category with id {command.CategoryId} was not found.");
        }

        var hasTransactions =
            await _transactionRepository.ExistsByCategoryIdAsync(
                category.Id,
                cancellationToken);

        if (hasTransactions)
        {
            throw new ConflictException(
                "The category cannot be deleted because it is used by one or more transactions.");
        }

        var hasMonthlyPlanLimits =
            await _monthlyPlanRepository.HasCategoryLimitAsync(
                category.Id,
                cancellationToken);

        if (hasMonthlyPlanLimits)
        {
            throw new ConflictException(
                "The category cannot be deleted because it is used by a monthly plan.");
        }

        _categoryRepository.Remove(category);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}