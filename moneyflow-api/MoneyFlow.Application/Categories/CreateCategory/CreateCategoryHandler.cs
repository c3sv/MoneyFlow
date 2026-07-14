using MoneyFlow.Application.Abstractions.Persistence;
using MoneyFlow.Application.Common.Exceptions;
using MoneyFlow.Domain.Categories;

namespace MoneyFlow.Application.Categories.CreateCategory;

public sealed class CreateCategoryHandler
{
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> HandleAsync(
        CreateCategoryCommand command,
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

        var category = new Category(
            command.UserId,
            command.Name,
            command.Type,
            command.Icon);

        var categoryExists =
            await _categoryRepository.ExistsByNameAndTypeAsync(
                category.UserId,
                category.Name,
                category.Type,
                cancellationToken);

        if (categoryExists)
        {
            throw new ConflictException(
                "A category with the same name and type already exists.");
        }

        await _categoryRepository.AddAsync(
            category,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}