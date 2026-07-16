using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.Categories;
using MoneyFlow.Application.Categories.CreateCategory;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/categories")]
public sealed class CategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler _createCategoryHandler;

    public CategoriesController(
        CreateCategoryHandler createCategoryHandler)
    {
        _createCategoryHandler = createCategoryHandler;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateCategoryResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
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