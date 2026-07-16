using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Categories;
using MoneyFlow.Application.Categories.CreateCategory;
using MoneyFlow.Application.Categories.GetCategories;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler _createCategoryHandler;
    private readonly GetCategoriesHandler _getCategoriesHandler;

    public CategoriesController(
        CreateCategoryHandler createCategoryHandler,
        GetCategoriesHandler getCategoriesHandler)
    {
        _createCategoryHandler = createCategoryHandler;
        _getCategoriesHandler = getCategoriesHandler;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<CategoryResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetCategoriesQuery(userId);

        var results = await _getCategoriesHandler.HandleAsync(
            query,
            cancellationToken);

        var response = results
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                category.Type,
                category.Icon))
            .ToList();

        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateCategoryResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CreateCategoryResponse>> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new CreateCategoryCommand(
            userId,
            request.Name,
            request.Type,
            request.Icon);

        var categoryId =
            await _createCategoryHandler.HandleAsync(
                command,
                cancellationToken);

        var response = new CreateCategoryResponse(categoryId);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }
}