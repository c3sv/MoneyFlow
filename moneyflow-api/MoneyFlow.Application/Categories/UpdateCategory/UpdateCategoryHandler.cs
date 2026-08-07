using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;

namespace MoneyFlow.Application.Categories.UpdateCategory;

public sealed class UpdateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandler(
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
        UpdateCategoryCommand command,
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

        category.UpdateDetails(
            command.Name,
            command.Icon);

        var categoryExists =
            await _categoryRepository
                .ExistsByNameAndTypeExcludingIdAsync(
                    category.UserId,
                    category.Name,
                    category.Type,
                    category.Id,
                    cancellationToken);

        if (categoryExists)
        {
            throw new ConflictException(
                "A category with the same name and type already exists.");
        }

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}