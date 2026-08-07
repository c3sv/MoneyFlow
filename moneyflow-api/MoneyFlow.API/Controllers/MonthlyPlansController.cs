using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyFlow.API.Common.Authentication;
using MoneyFlow.API.Contracts.MonthlyPlans;
using MoneyFlow.Application.MonthlyPlans.AddCategoryLimit;
using MoneyFlow.Application.MonthlyPlans.CreateMonthlyPlan;
using MoneyFlow.Application.MonthlyPlans.GetMonthlyPlans;
using MoneyFlow.Application.MonthlyPlans.DeleteCategoryLimit;
using MoneyFlow.Application.MonthlyPlans.DeleteMonthlyPlan;
using MoneyFlow.Application.MonthlyPlans.GetMonthlyPlanById;
using MoneyFlow.Application.MonthlyPlans.UpdateCategoryLimit;
using MoneyFlow.Application.MonthlyPlans.UpdateMonthlyPlan;

namespace MoneyFlow.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/monthly-plans")]
public sealed class MonthlyPlansController : ControllerBase
{
    private readonly CreateMonthlyPlanHandler
        _createMonthlyPlanHandler;

    private readonly AddCategoryLimitHandler
        _addCategoryLimitHandler;

    private readonly GetMonthlyPlansHandler _getMonthlyPlansHandler;
    private readonly GetMonthlyPlanByIdHandler _getMonthlyPlanByIdHandler;
    private readonly UpdateMonthlyPlanHandler _updateMonthlyPlanHandler;
    private readonly DeleteMonthlyPlanHandler _deleteMonthlyPlanHandler;
    private readonly UpdateCategoryLimitHandler _updateCategoryLimitHandler;
    private readonly DeleteCategoryLimitHandler _deleteCategoryLimitHandler;
    public MonthlyPlansController(
        CreateMonthlyPlanHandler createMonthlyPlanHandler,
        GetMonthlyPlansHandler getMonthlyPlansHandler,
        GetMonthlyPlanByIdHandler getMonthlyPlanByIdHandler,
        UpdateMonthlyPlanHandler updateMonthlyPlanHandler,
        DeleteMonthlyPlanHandler deleteMonthlyPlanHandler,
        AddCategoryLimitHandler addCategoryLimitHandler,
        UpdateCategoryLimitHandler updateCategoryLimitHandler,
        DeleteCategoryLimitHandler deleteCategoryLimitHandler)
    {
        _createMonthlyPlanHandler = createMonthlyPlanHandler;
        _getMonthlyPlansHandler = getMonthlyPlansHandler;
        _getMonthlyPlanByIdHandler = getMonthlyPlanByIdHandler;
        _updateMonthlyPlanHandler = updateMonthlyPlanHandler;
        _deleteMonthlyPlanHandler = deleteMonthlyPlanHandler;
        _addCategoryLimitHandler = addCategoryLimitHandler;
        _updateCategoryLimitHandler = updateCategoryLimitHandler;
        _deleteCategoryLimitHandler = deleteCategoryLimitHandler;
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(CreateMonthlyPlanResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status409Conflict)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CreateMonthlyPlanResponse>> Create(
        [FromBody] CreateMonthlyPlanRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new CreateMonthlyPlanCommand(
            userId,
            request.Month,
            request.Year,
            request.ExpectedIncome,
            request.TargetSavings,
            request.TotalSpendingLimit,
            request.Currency);

        var monthlyPlanId =
            await _createMonthlyPlanHandler.HandleAsync(
                command,
                cancellationToken);

        var response =
            new CreateMonthlyPlanResponse(monthlyPlanId);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }

    [HttpPost("{monthlyPlanId:long}/category-limits")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddCategoryLimit(
        long monthlyPlanId,
        [FromBody] AddCategoryLimitRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new AddCategoryLimitCommand(
            userId,
            monthlyPlanId,
            request.CategoryId,
            request.LimitAmount);

        await _addCategoryLimitHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
    
    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<MonthlyPlanResponse>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<MonthlyPlanResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetMonthlyPlansQuery(userId);

        var results = await _getMonthlyPlansHandler.HandleAsync(
            query,
            cancellationToken);

        var response = results
            .Select(ToResponse)
            .ToList();
        
        return Ok(response);
    }
    [HttpGet("{monthlyPlanId:long}")]
    [ProducesResponseType(
        typeof(MonthlyPlanResponse),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MonthlyPlanResponse>> GetById(
        long monthlyPlanId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var query = new GetMonthlyPlanByIdQuery(
            userId,
            monthlyPlanId);

        var result = await _getMonthlyPlanByIdHandler.HandleAsync(
            query,
            cancellationToken);

        return Ok(ToResponse(result));
    }
    
    [HttpPut("{monthlyPlanId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        long monthlyPlanId,
        [FromBody] UpdateMonthlyPlanRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new UpdateMonthlyPlanCommand(
            userId,
            monthlyPlanId,
            request.ExpectedIncome,
            request.TargetSavings,
            request.TotalSpendingLimit);

        await _updateMonthlyPlanHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
    
    [HttpDelete("{monthlyPlanId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        long monthlyPlanId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new DeleteMonthlyPlanCommand(
            userId,
            monthlyPlanId);

        await _deleteMonthlyPlanHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
    [HttpPut(
        "{monthlyPlanId:long}/category-limits/{categoryId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCategoryLimit(
        long monthlyPlanId,
        long categoryId,
        [FromBody] UpdateCategoryLimitRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new UpdateCategoryLimitCommand(
            userId,
            monthlyPlanId,
            categoryId,
            request.LimitAmount);

        await _updateCategoryLimitHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
    [HttpDelete(
        "{monthlyPlanId:long}/category-limits/{categoryId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategoryLimit(
        long monthlyPlanId,
        long categoryId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var command = new DeleteCategoryLimitCommand(
            userId,
            monthlyPlanId,
            categoryId);

        await _deleteCategoryLimitHandler.HandleAsync(
            command,
            cancellationToken);

        return NoContent();
    }
    private static MonthlyPlanResponse ToResponse(
        MonthlyPlanResult monthlyPlan)
    {
        return new MonthlyPlanResponse(
            monthlyPlan.Id,
            monthlyPlan.Month,
            monthlyPlan.Year,
            monthlyPlan.ExpectedIncome,
            monthlyPlan.TargetSavings,
            monthlyPlan.TotalSpendingLimit,
            monthlyPlan.Currency,
            monthlyPlan.CreatedAt,
            monthlyPlan.CategoryLimits
                .Select(limit => new CategoryLimitResponse(
                    limit.Id,
                    limit.CategoryId,
                    limit.LimitAmount))
                .ToList());
    }
}